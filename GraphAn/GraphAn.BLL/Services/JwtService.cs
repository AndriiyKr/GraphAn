// <copyright file="JwtService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using GraphAn.BLL.Interfaces;
    using GraphAn.DAL.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Сервіс для генерації JWT токенів.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        public JwtService(ILogger<JwtService> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Генерує JWT токен для користувача.
        /// </summary>
        /// <param name="user">Об'єкт користувача.</param>
        /// <returns>JWT токен у вигляді рядка.</returns>
        public string GenerateToken(User user)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET");

            if (string.IsNullOrWhiteSpace(secret))
            {
                this.logger.LogError("JWT_SECRET відсутній у змінних середовища.");
                throw new InvalidOperationException("JWT_SECRET is not configured.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials,
                claims: claims);

            this.logger.LogInformation("JWT токен згенеровано для користувача: {Email}", user.Email);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}