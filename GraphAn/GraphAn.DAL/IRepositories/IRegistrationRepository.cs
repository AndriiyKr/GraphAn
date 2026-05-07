// <copyright file="IRegistrationRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Інтерфейс репозиторію для тимчасових реєстрацій.
    /// </summary>
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        /// <summary>
        /// Перевіряє, чи існує тимчасова реєстрація з вказаним email.
        /// </summary>
        /// <param name="email">Електронна пошта.</param>
        /// <returns><see langword="true"/>, якщо реєстрація існує, інакше <see langword="false"/>.</returns>
        Task<bool> IfEmailExistsAsync(string email);

        /// <summary>
        /// Отримує тимчасову реєстрацію за email.
        /// </summary>
        /// <param name="email">Електронна пошта.</param>
        /// <returns>Об'єкт <see cref="Registration"/>, якщо знайдено, інакше <see langword="null"/>.</returns>
        Task<Registration?> GetByEmailAsync(string email);
    }
}