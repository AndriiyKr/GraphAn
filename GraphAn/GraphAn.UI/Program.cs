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
    using GraphAn.DAL.Repositories;
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
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<ProjectRepository>();
            builder.Services.AddScoped<RegistrationRepository>();

            try
            {
                Log.Information("Запуск веб-додатка...");

                // Add services to the container.
                builder.Services.AddControllersWithViews();

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
