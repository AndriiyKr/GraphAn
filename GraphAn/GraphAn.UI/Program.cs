// <copyright file="Program.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI
{
    using System.Text;
    using DotNetEnv;
    using GraphAn.BLL.Interfaces;
    using GraphAn.BLL.Services;
    using GraphAn.DAL.Context;
    using GraphAn.DAL.Models;
    using GraphAn.DAL.Repositories;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Serilog;

    /// <summary>
    /// Забезпечує вхідну точку у програму.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///  Головна точка входу у програму.
        /// </summary>
        /// <param name="args">Додаткові аргументи.</param>
        public static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);

            Env.Load();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .CreateLogger();

            builder.Host.UseSerilog();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IGraphMetricsService, GraphMetricsService>();
            builder.Services.AddScoped<IAlgorithmService, AlgorithmService>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<ProjectRepository>();
            builder.Services.AddScoped<RegistrationRepository>();

            try
            {
                Log.Information("Запуск веб-додатка...");

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                // 1. Додаємо Identity з нашою моделлю User та роллю IdentityRole<Guid>
                builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    // Налаштування паролів (можна залишити стандартні або задати свої)
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Налаштування блокування (lockout)
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;

                    // Підтвердження email (можна увімкнути пізніше)
                    options.SignIn.RequireConfirmedEmail = false;

                    // Ім'я користувача – email не обов'язково співпадає
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>() // використовуємо наш DbContext
                .AddDefaultTokenProviders();                 // для скидання пароля, підтвердження email

                // 2. Налаштовуємо cookie-автентифікацію (вона використовується за замовчуванням)
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.LoginPath = "/Account/Login";          // шлях до сторінки входу
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.SlidingExpiration = true;
                });

                // 3. Якщо ви хочете використовувати авторизацію через куки явно, можна додати:
                builder.Services.AddAuthentication();
                builder.Services.AddAuthorization();

                // Configure AppDbContext using CONNECTION_STRING from environment or configuration
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                    ?? builder.Configuration.GetConnectionString("Supabase");

                if (string.IsNullOrEmpty(connectionString))
                {
                    Log.Error("CONNECTION_STRING не встановлено. Перевірте .env або конфігурацію.");
                    throw new InvalidOperationException("CONNECTION_STRING is not set.");
                }

                builder.Services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);

                    if (builder.Environment.IsDevelopment())
                    {
                        options.EnableSensitiveDataLogging();
                    }
                });

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    try
                    {
                        // Намагаємося перевірити, чи доступна база
                        if (dbContext.Database.CanConnect())
                        {
                            Log.Information("Успішне підключення до бази даних Supabase!");
                        }
                        else
                        {
                            Log.Error("Не вдалося підключитися до бази даних.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Помилка при перевірці підключення");
                    }
                }

                app.UseSerilogRequestLogging();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");

                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                // app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Додаток завершив роботу некоректно");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
