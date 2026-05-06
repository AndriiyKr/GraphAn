// <copyright file="AuthController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using System.Threading.Tasks;
    using GraphAn.BLL.Interfaces;
    using GraphAn.DAL.Models;
    using GraphAn.UI.ViewModels;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для автентифікації та реєстрації користувачів.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService emailService;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="emailService">Сервіс для роботи з email та автентифікацією.</param>
        /// <param name="userManager">Менеджер користувачів Identity.</param>
        /// <param name="signInManager">Менеджер входу Identity.</param>
        public AuthController(
            IEmailService emailService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.emailService = emailService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Створює тимчасову реєстрацію та надсилає код підтвердження на email.
        /// </summary>
        /// <param name="request">Дані користувача для реєстрації.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> StartRegister([FromBody] UserRequest request)
        {
            var result = await this.emailService.StartRegistrationAsync(request.Email, request.Password, request.UserName);
            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }

        /// <summary>
        /// Підтверджує реєстрацію користувача за допомогою коду з email.
        /// </summary>
        /// <param name="request">Email та код підтвердження.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmRegister([FromBody] CodeConfirmRequest request)
        {
            var result = await this.emailService.ConfirmRegistrationAsync(request.Email, request.Code);
            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            // Після успішного підтвердження автоматично виконуємо вхід користувача
            var user = await this.userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                await this.signInManager.SignInAsync(user, isPersistent: false);
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }

        /// <summary>
        /// Виконує вхід користувача за email або іменем користувача і паролем.
        /// </summary>
        /// <param name="request">Дані для входу.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="UnauthorizedObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            // Визначаємо, чи введено email або username
            var user = request.Login.Contains('@')
                ? await this.userManager.FindByEmailAsync(request.Login)
                : await this.userManager.FindByNameAsync(request.Login);

            if (user == null)
            {
                return this.Unauthorized(new ErrorResponse { Message = "Невірний логін або пароль." });
            }

            if (!user.EmailConfirmed)
            {
                return this.Unauthorized(new ErrorResponse { Message = "Підтвердіть email за допомогою коду, надісланого під час реєстрації." });
            }

            var result = await this.signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return this.Ok(new SuccessResponse { Message = "Вхід виконано." });
            }

            if (result.IsLockedOut)
            {
                return this.Unauthorized(new ErrorResponse { Message = "Акаунт заблоковано на 5 хвилин через багато невдалих спроб." });
            }

            return this.Unauthorized(new ErrorResponse { Message = "Невірний логін або пароль." });
        }

        /// <summary>
        /// Виконує вихід користувача з системи.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх.
        /// </returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.Ok(new SuccessResponse { Message = "Вихід виконано." });
        }

        /// <summary>
        /// Надсилає на email посилання для скидання пароля.
        /// </summary>
        /// <param name="request">Email користувача.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordRequest request)
        {
            var resetLinkGenerator = (string email, string token) =>
                $"{this.Request.Scheme}://{this.Request.Host}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            var result = await this.emailService.ForgotPasswordAsync(request.Email, resetLinkGenerator);
            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }

        /// <summary>
        /// Скидає пароль користувача за токеном.
        /// </summary>
        /// <param name="request">Дані для скидання пароля.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequest request)
        {
            var result = await this.emailService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }

        /// <summary>
        /// Отримує інформацію про поточного авторизованого користувача.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> з даними користувача,
        /// або <see cref="UnauthorizedResult"/> якщо користувач не авторизований.
        /// </returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.Unauthorized();
            }

            return this.Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.EmailConfirmed,
                user.CreatedAt,
            });
        }
    }
}