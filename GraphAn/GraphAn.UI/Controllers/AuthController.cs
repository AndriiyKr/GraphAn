// <copyright file="AuthController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using GraphAn.BLL.Interfaces;
    using GraphAn.UI.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для автентифікації та реєстрації користувачів.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService emailService;
        private readonly IJwtService jwtService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="emailService">Сервіс для роботи з email та автентифікацією.</param>
        /// <param name="jwtService">Сервіс для роботи з jwt.</param>
        public AuthController(IEmailService emailService, IJwtService jwtService)
        {
            this.emailService = emailService;
            this.jwtService = jwtService;
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
            var result = await this.emailService.StartRegistrationAsync(request.Email, request.Password, request.Username);

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
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            var result = await this.emailService.UserLoginAsync(request.Email, request.Password, request.Username);

            if (!result.Success)
            {
                return this.Unauthorized(new ErrorResponse { Message = result.Message });
            }

            var token = this.jwtService.GenerateToken(result.User!);

            return this.Ok(new LoginResponse
            {
                UserId = result.User!.Id,
                Username = result.User!.Username,
                Email = result.User!.Email,
                Token = token,
            });
        }
    }
}
