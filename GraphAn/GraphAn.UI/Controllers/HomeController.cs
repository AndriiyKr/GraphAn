// <copyright file="HomeController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для відображення сторінок застосунку.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
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
        /// <returns><see cref="ViewResult"/> зі сторінкою редактора проекту.</returns>
        [HttpGet]
        public IActionResult Project(Guid id)
        {
            this.logger.LogInformation("Відкрито сторінку редактора проєкту: {Id}", id);
            return this.View();
        }
    }
}