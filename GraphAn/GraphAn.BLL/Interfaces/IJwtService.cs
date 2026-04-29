// <copyright file="IJwtService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Інтерфейс сервісу для роботи з JWT токенами.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Генерує JWT токен для користувача.
        /// </summary>
        /// <param name="user">Об'єкт користувача.</param>
        /// <returns>JWT токен у вигляді рядка.</returns>
        string GenerateToken(User user);
    }
}