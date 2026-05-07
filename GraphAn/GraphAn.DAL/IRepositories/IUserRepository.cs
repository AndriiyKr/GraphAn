// <copyright file="IUserRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Інтерфейс репозиторію для роботи з користувачами.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Отримує користувача за електронною поштою.
        /// </summary>
        /// <param name="email">Електронна пошта.</param>
        /// <returns>Користувач, якщо знайдено, інакше <see langword="null"/>.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Отримує користувача за ім'ям користувача.
        /// </summary>
        /// <param name="username">Ім'я користувача.</param>
        /// <returns>Користувач, якщо знайдено, інакше <see langword="null"/>.</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Перевіряє, чи існує користувач з вказаною електронною поштою.
        /// </summary>
        /// <param name="email">Електронна пошта.</param>
        /// <returns><see langword="true"/>, якщо користувач існує, інакше <see langword="false"/>.</returns>
        Task<bool> IfEmailExistsAsync(string email);

        /// <summary>
        /// Перевіряє, чи існує користувач з вказаним ім'ям.
        /// </summary>
        /// <param name="username">Ім'я користувача.</param>
        /// <returns><see langword="true"/>, якщо користувач існує, інакше <see langword="false"/>.</returns>
        Task<bool> IfUsernameExistsAsync(string username);
    }
}