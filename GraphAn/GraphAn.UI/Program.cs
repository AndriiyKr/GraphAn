// <copyright file="Program.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI
{
    using System.Text;
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

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .CreateLogger();

            builder.Host.UseSerilog();

            try
            {
                Log.Information("Запуск веб-додатка...");

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                var app = builder.Build();

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
