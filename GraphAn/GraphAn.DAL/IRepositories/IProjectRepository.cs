// <copyright file="IProjectRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Інтерфейс репозиторію для роботи з проєктами.
    /// </summary>
    public interface IProjectRepository : IGenericRepository<Project>
    {
        /// <summary>
        /// Отримує всі проєкти користувача.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>Список проєктів, що належать вказаному користувачеві.</returns>
        Task<List<Project>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Отримує проєкт за ідентифікатором проєкту та користувача.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проєкту.</param>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>Проєкт, якщо знайдено, інакше <see langword="null"/>.</returns>
        Task<Project?> GetByIdAndUserIdAsync(Guid projectId, Guid userId);

        /// <summary>
        /// Отримує список проєктів користувача без даних графа (лише метадані).
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>Список проєктів без поля GraphData.</returns>
        Task<List<Project>> GetProjectsByUserIdAsync(Guid userId);
    }
}