// <copyright file="UserRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Context;
    using GraphAn.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a User repository.
    /// </summary>
    public class UserRepository : GenericRepository<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations. Cannot be <see langword="null"/>.</param>
        public UserRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Отримання користувача за email.
        /// </summary>
        /// <param name="email">email користувача.</param>
        /// <returns> <see cref="User"/> якщо знайдено; інакше <see langword="null"/>.</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await this.DbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Перевірка існування користувача з переданим email.
        /// </summary>
        /// <param name="email">email користувача.</param>
        /// <returns> <see langword="true"/> якщо знайдено; інакше <see langword="false"/>.</returns>
        public async Task<bool> IfEmailExistsAsync(string email)
        {
            return await this.DbSet.AnyAsync(u => u.Email == email);
        }
    }
}