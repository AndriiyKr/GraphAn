// <copyright file="ProjectRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Context;
    using GraphAn.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a Project repository.
    /// </summary>
    public class ProjectRepository : GenericRepository<Project>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations. Cannot be <see langword="null"/>.</param>
        public ProjectRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Отримує всі проекти користувача за його ідентифікатором.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>Список проектів користувача.</returns>
        public async Task<List<Project>> GetByUserIdAsync(Guid userId)
        {
            return await this.DbSet
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Отримує проект за його ідентифікатором та ідентифікатором користувача.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns><see cref="Project"/> якщо знайдено; інакше <see langword="null"/>.</returns>
        public async Task<Project?> GetByIdAndUserIdAsync(Guid projectId, Guid userId)
        {
            return await this.DbSet
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        }

        /// <summary>
        /// Отримує список проектів користувача без даних графа.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <returns>Список проектів користувача.</returns>
        public async Task<List<Project>> GetProjectsByUserIdAsync(Guid userId)
        {
            return await this.DbSet
                .Where(p => p.UserId == userId)
                .Select(p => new Project
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Name = p.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                })
                .ToListAsync();
        }
    }
}