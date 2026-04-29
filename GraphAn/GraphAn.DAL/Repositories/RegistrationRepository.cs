// <copyright file="RegistrationRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Context;
    using GraphAn.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a Registration repository.
    /// </summary>
    public class RegistrationRepository : GenericRepository<Registration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations. Cannot be <see langword="null"/>.</param>
        public RegistrationRepository(AppDbContext context)
            : base(context)
        {
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