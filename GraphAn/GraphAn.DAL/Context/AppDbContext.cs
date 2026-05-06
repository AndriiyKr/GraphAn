// <copyright file="AppDbContext.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Context
{
    using DotNetEnv;
    using GraphAn.DAL.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Основний контекст бази даних додатка для взаємодії з PostgreSQL через Entity Framework Core.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// Статичне поле для кешування рядка підключення.
        /// </summary>
        private static string? cachedConnection;

        /// <summary>
        /// Логер для запису подій всередині шару доступу до даних.
        /// </summary>
        private readonly ILogger<AppDbContext>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// Конструктор для використання в системі Dependency Injection.
        /// Автоматично отримує налаштовані параметри контексту та сервіс логування.
        /// </summary>
        /// <param name="logger">Сервіс логування.</param>
        /// <param name="options">Налаштування конфігурації DbContext.</param>
        public AppDbContext(ILogger<AppDbContext> logger, DbContextOptions<AppDbContext> options)
            : base(options)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">Налаштування конфігурації DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        public AppDbContext()
        {
        }

        /// <summary>
        /// Gets or sets набір проєктів.
        /// </summary>
        public DbSet<Project> Projects { get; set; } = null!;

        /// <summary>
        /// Gets or sets набір реєстрацій.
        /// </summary>
        public DbSet<Registration> Registrations { get; set; } = null!;

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

        /// <summary>
        /// Конфігурація моделі та зв'язків між сутностями.
        /// </summary>
        /// <param name="modelBuilder">Builder для моделі EF.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

            modelBuilder.Entity<User>(entity =>
            {
                // Первинний ключ
                entity.Property(u => u.Id).HasColumnName("id");

                // Унікальне ім'я
                entity.Property(u => u.UserName).HasColumnName("username");
                entity.HasIndex(u => u.NormalizedUserName).HasDatabaseName("ix_users_normalized_username");

                // Email
                entity.Property(u => u.Email).HasColumnName("email");
                entity.HasIndex(u => u.NormalizedEmail).HasDatabaseName("ix_users_normalized_email");

                // Інші поля Identity
                entity.Property(u => u.NormalizedUserName).HasColumnName("normalized_username");
                entity.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
                entity.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(u => u.PhoneNumber).HasColumnName("phone_number");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                entity.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");

                // Користувацьке поле
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");

                // Зв'язок з проектами
                entity.HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<Registration>().ToTable("registrations");
        }
    }
}