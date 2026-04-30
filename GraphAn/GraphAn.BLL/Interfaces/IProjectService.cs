// <copyright file="IProjectService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Інтерфейс сервісу для роботи з проектами.
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Створює новий порожній проект.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="name">Назва проекту.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>ProjectId</c> — ідентифікатор створеного проекту.
        /// </returns>
        Task<(bool Success, string Message, Guid? ProjectId)> CreateProjectAsync(Guid userId, string? name);

        /// <summary>
        /// Зберігає назву та дані графа існуючого проекту.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <param name="name">Нова назва проекту.</param>
        /// <param name="graphData">Дані графа у форматі JSON.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> SaveProjectAsync(Guid userId, Guid projectId, string? name, string? graphData);

        /// <summary>
        /// Отримує список проектів користувача без даних графа.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Projects</c> — список проектів користувача.
        /// </returns>
        Task<(bool Success, string Message, List<Project>? Projects)> GetProjectsAsync(Guid userId);

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
        Task<(bool Success, string Message, Project? Project)> GetProjectAsync(Guid userId, Guid projectId);

        /// <summary>
        /// Видаляє проект користувача за ідентифікатором.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> DeleteProjectAsync(Guid userId, Guid projectId);
    }
}