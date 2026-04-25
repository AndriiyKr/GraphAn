// <copyright file="AppDbContext.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Context
{
    using System;
    using DotNetEnv;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using GraphAn.DAL.Models;

    /// <summary>
    /// Основний контекст бази даних додатка для взаємодії з PostgreSQL через Entity Framework Core.
    /// Забезпечує керування підключенням, транзакціями та мапінг об'єктів на таблиці БД.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Статичне поле для кешування рядка підключення, щоб уникнути повторного зчитування
        ///  конфігураційних файлів при кожному створенні екземпляра контексту.
        /// </summary>
        private static string? cachedConnection;

        /// <summary>
        /// Логер для запису подій, що відбуваються всередині шару доступу до даних (SQL-запити, помилки БД).
        /// </summary>
        private readonly ILogger<AppDbContext>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// Конструктор для використання в системі Dependency Injection.
        /// Автоматично отримує налаштовані параметри контексту та сервіс логування.
        /// </summary>
        /// <param name="logger">Сервіс логування для запису операцій контексту.</param>
        /// <param name="options">Налаштування конфігурації DbContext (провайдер БД, рядок підключення тощо).</param>
        public AppDbContext(ILogger<AppDbContext> logger, DbContextOptions<AppDbContext> options)
            : base(options)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// Спрощений конструктор для ініціалізації контексту з параметрами, але без сервісу логування.
        /// </summary>
        /// <param name="options">Налаштування конфігурації DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// Конструктор без параметрів.
        /// </summary>
        public AppDbContext()
        {
        }

        /// <summary>
        /// Конфігурує параметри підключення до бази даних, якщо вони не були задані ззовні.
        /// Використовує бібліотеку DotNetEnv для зчитування змінних середовища та налаштовує
        ///  підключення до PostgreSQL з розширеним логуванням для режиму розробки.
        /// </summary>
        /// <param name="optionsBuilder">Будівельник параметрів, що дозволяє вибрати провайдер БД та налаштувати поведінку контексту.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (cachedConnection == null)
                {
                    Env.Load();
                    cachedConnection = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                    if (string.IsNullOrEmpty(cachedConnection))
                    {
                        this.logger?.LogError("CONNECTION_STRING не знайдено у .env");
                    }
                }

                optionsBuilder.UseNpgsql(cachedConnection);

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableSensitiveDataLogging();
                }
            }
        }

        // Added DbSet properties for EF Core
        /// <summary>
        /// Набір користувачів.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Набір проєктів.
        /// </summary>
        public DbSet<Project> Projects { get; set; } = null!;

        /// <summary>
        /// Набір реєстрацій.
        /// </summary>
        public DbSet<Registration> Registrations { get; set; } = null!;

        /// <summary>
        /// Конфігурація моделі та зв'язків між сутностями.
        /// </summary>
        /// <param name="modelBuilder">Builder для моделі EF.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування сутності User
            modelBuilder.Entity<User>(entity =>
            {
                // Робимо Email унікальним
                entity.HasIndex(u => u.Email)
                    .IsUnique();

                // Зв'язок між User і Project (один-до-багатьох)
                entity.HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Мапінг назв таблиць на малі літери
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<Registration>().ToTable("registrations");
        }
    }
}