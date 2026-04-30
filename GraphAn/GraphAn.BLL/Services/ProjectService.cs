// <copyright file="ProjectService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using GraphAn.BLL.Interfaces;
    using GraphAn.DAL.Models;
    using GraphAn.DAL.Repositories;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Сервіс для роботи з проєктами.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> logger;
        private readonly ProjectRepository projectRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        /// <param name="projectRepository">Об'єкт репозиторію проєктів.</param>
        public ProjectService(
            ILogger<ProjectService> logger,
            ProjectRepository projectRepository)
        {
            this.logger = logger;
            this.projectRepository = projectRepository;
        }

        /// <summary>
        /// Створює новий порожній проєкт.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="name">Назва проєкту.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>ProjectId</c> — ідентифікатор створеного проєкту.
        /// </returns>
        public async Task<(bool Success, string Message, Guid? ProjectId)> CreateProjectAsync(Guid userId, string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Новий проєкт";
            }

            if (name.Length > 100)
            {
                this.logger.LogWarning("Передано занадто довгу назву проєкту для користувача: {UserId}", userId);
                return (false, "Назва проєкту не може перевищувати 100 символів", null);
            }

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                GraphData = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await this.projectRepository.AddAsync(project);

            this.logger.LogInformation("Проєкт '{Name}' створено для користувача: {UserId}", name, userId);
            return (true, "Проєкт успішно створено", project.Id);
        }

        /// <summary>
        /// Зберігає назву та дані графа існуючого проєкту.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="projectId">Ідентифікатор проєкту.</param>
        /// <param name="name">Нова назва проєкту.</param>
        /// <param name="graphData">Дані графа у форматі JSON.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        public async Task<(bool Success, string Message)> SaveProjectAsync(Guid userId, Guid projectId, string? name, string? graphData)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Length > 100)
            {
                this.logger.LogWarning("Передано занадто довгу назву проєкту для проєкту: {ProjectId}", projectId);
                return (false, "Назва проєкту не може перевищувати 100 символів");
            }

            var project = await this.projectRepository.GetByIdAndUserIdAsync(projectId, userId);

            if (project == null)
            {
                this.logger.LogWarning("Проєкт {ProjectId} не знайдено для користувача: {UserId}", projectId, userId);
                return (false, "Проєкт не знайдено");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                project.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(graphData))
            {
                project.GraphData = graphData;
            }

            project.UpdatedAt = DateTime.UtcNow;

            await this.projectRepository.UpdateAsync(project);

            this.logger.LogInformation("Проєкт '{ProjectId}' збережено для користувача: {UserId}", projectId, userId);
            return (true, "Проєкт успішно збережено");
        }

        /// <summary>
        /// Отримує список проектів користувача без даних графа.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Projects</c> — список проектів користувача.
        /// </returns>
        public async Task<(bool Success, string Message, List<Project>? Projects)> GetProjectsAsync(Guid userId)
        {
            var projects = await this.projectRepository.GetProjectsByUserIdAsync(userId);

            this.logger.LogInformation("Отримано {Count} проєктів для користувача: {UserId}", projects.Count, userId);
            return (true, "Проєкти успішно отримано", projects);
        }

        /// <summary>
        /// Отримує проект з даними графа за ідентифікатором.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Project</c> — проект з даними графа.
        /// </returns>
        public async Task<(bool Success, string Message, Project? Project)> GetProjectAsync(Guid userId, Guid projectId)
        {
            var project = await this.projectRepository.GetByIdAndUserIdAsync(projectId, userId);

            if (project == null)
            {
                this.logger.LogWarning("Проєкт {ProjectId} не знайдено для користувача: {UserId}", projectId, userId);
                return (false, "Проєкт не знайдено", null);
            }

            this.logger.LogInformation("Проєкт {ProjectId} отримано для користувача: {UserId}", projectId, userId);
            return (true, "Проєкт успішно отримано", project);
        }

        /// <summary>
        /// Видаляє проект користувача за ідентифікатором.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        public async Task<(bool Success, string Message)> DeleteProjectAsync(Guid userId, Guid projectId)
        {
            var project = await this.projectRepository.GetByIdAndUserIdAsync(projectId, userId);

            if (project == null)
            {
                this.logger.LogWarning("Проєкт {ProjectId} не знайдено для користувача: {UserId}", projectId, userId);
                return (false, "Проєкт не знайдено");
            }

            await this.projectRepository.DeleteAsync(project);

            this.logger.LogInformation("Проєкт {ProjectId} видалено для користувача: {UserId}", projectId, userId);
            return (true, "Проєкт успішно видалено");
        }
    }
}