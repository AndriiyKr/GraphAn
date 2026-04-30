// <copyright file="ProjectController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using System.Security.Claims;
    using GraphAn.BLL.Interfaces;
    using GraphAn.UI.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для роботи з проєктами.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="projectService">Сервіс для роботи з проєктами.</param>
        public ProjectController(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        /// <summary>
        /// Створює новий порожній проєкт.
        /// </summary>
        /// <param name="request">Дані для створення проєкту.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з ідентифікатором створеного проєкту,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.Unauthorized(new ErrorResponse { Message = "Користувача не авторизовано" });
            }

            var result = await this.projectService.CreateProjectAsync(userId, request.Name);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new CreateProjectResponse
            {
                ProjectId = result.ProjectId!.Value,
                Message = result.Message,
            });
        }

        /// <summary>
        /// Зберігає дані існуючого проєкту.
        /// </summary>
        /// <param name="request">Дані для збереження проєкту.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPut("save")]
        public async Task<IActionResult> SaveProject([FromBody] SaveProjectRequest request)
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.Unauthorized(new ErrorResponse { Message = "Користувача не авторизовано" });
            }

            var result = await this.projectService.SaveProjectAsync(
                userId, request.ProjectId, request.Name, request.GraphData);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }

        /// <summary>
        /// Отримує список проєктів користувача без даних графа.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> зі списком проєктів,
        /// або <see cref="UnauthorizedObjectResult"/> з описом помилки.
        /// </returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetProjects()
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.Unauthorized(new ErrorResponse { Message = "Користувача не авторизовано" });
            }

            var result = await this.projectService.GetProjectsAsync(userId);

            return this.Ok(new ProjectListResponse
            {
                Projects = result.Projects!.Select(p => new ProjectShortResponse
                {
                    ProjectId = p.Id,
                    Name = p.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                }).ToList(),
            });
        }

        /// <summary>
        /// Отримує проєкт з даними графа за ідентифікатором.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проєкту.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з даними проєкту,
        /// або <see cref="NotFoundObjectResult"/> з описом помилки.
        /// </returns>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(Guid projectId)
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.Unauthorized(new ErrorResponse { Message = "Користувача не авторизовано" });
            }

            var result = await this.projectService.GetProjectAsync(userId, projectId);

            if (!result.Success)
            {
                return this.NotFound(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new ProjectResponse
            {
                ProjectId = result.Project!.Id,
                Name = result.Project!.Name,
                GraphData = result.Project!.GraphData,
                CreatedAt = result.Project!.CreatedAt,
                UpdatedAt = result.Project!.UpdatedAt,
            });
        }

        /// <summary>
        /// Видаляє проект користувача за ідентифікатором.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проекту.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з повідомленням про успіх,
        /// або <see cref="NotFoundObjectResult"/> з описом помилки.
        /// </returns>
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return this.Unauthorized(new ErrorResponse { Message = "Користувача не авторизовано" });
            }

            var result = await this.projectService.DeleteProjectAsync(userId, projectId);

            if (!result.Success)
            {
                return this.NotFound(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(new SuccessResponse { Message = result.Message });
        }
    }
}