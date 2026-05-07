// <copyright file="HomeController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using System.Security.Claims;
    using GraphAn.BLL.Interfaces;
    using GraphAn.UI.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для відображення сторінок застосунку.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IProjectService projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        /// <param name="projectService">Сервіс для роботи з проєктами.</param>
        public HomeController(ILogger<HomeController> logger, IProjectService projectService)
        {
            this.logger = logger;
            this.projectService = projectService;
        }

        /// <summary>
        /// Відображає головну сторінку застосунку.
        /// </summary>
        /// <returns><see cref="ViewResult"/> з головною сторінкою.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            this.logger.LogInformation("Відкрито головну сторінку");
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку входу у акаунт.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою входу.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            this.logger.LogInformation("Відкрито сторінку входу");
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку реєстрації.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою реєстрації.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            this.logger.LogInformation("Відкрито сторінку реєстрації");
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку для запиту відновлення пароля.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою відновлення пароля.</returns>
        [HttpGet("home/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку підтвердження електронної пошти.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою підтвердження.</returns>
        [HttpGet]
        public IActionResult Confirm()
        {
            this.logger.LogInformation("Відкрито сторінку підтвердження email");
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку для скидання пароля.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="token">Токен скидання пароля.</param>
        /// <returns><see cref="ViewResult"/> зі сторінкою скидання пароля.</returns>
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string email, string token)
        {
            // Передати email і token у View через ViewBag або модель
            this.ViewBag.Email = email;
            this.ViewBag.Token = token;
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку зі списком проектів користувача.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою списку проектів.</returns>
        [HttpGet]
        public IActionResult Projects()
        {
            this.logger.LogInformation("Відкрито сторінку списку проєктів");
            return this.View();
        }

        /// <summary>
        /// Відображає сторінку редактора проекту.
        /// </summary>
        /// <param name="id">Ідентифікатор проекту.</param>
        /// <returns>
        /// <see cref="ViewResult"/> зі сторінкою редактора проекту,
        /// або <see cref="RedirectResult"/> на сторінку логіну, якщо користувач не авторизований,
        /// або <see cref="NotFoundResult"/> якщо проект не знайдено.
        /// </returns>
        public async Task<IActionResult> Project(Guid id)
        {
            // Отримуємо ідентифікатор поточного користувача з Claims
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.RedirectToAction("Login", "Home");
            }

            // Викликаємо сервіс для отримання проєкту
            var result = await this.projectService.GetProjectAsync(userId, id);
            if (!result.Success || result.Project == null)
            {
                return this.NotFound();
            }

            var viewModel = new ProjectViewModel
            {
                ProjectId = result.Project.Id,
                Name = result.Project.Name,
                GraphData = result.Project.GraphData,
            };

            return this.View(viewModel);
        }

        /// <summary>
        /// Відображає сторінку політики конфіденційності.
        /// </summary>
        /// <returns><see cref="ViewResult"/> зі сторінкою конфіденційності.</returns>
        [HttpGet]
        public IActionResult Privacy()
        {
            return this.View();
        }
    }
}