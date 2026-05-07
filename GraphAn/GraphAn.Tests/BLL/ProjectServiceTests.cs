// <copyright file="ProjectServiceTests.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Tests.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraphAn.BLL.Services;
    using GraphAn.DAL.Models;
    using GraphAn.DAL.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using FluentAssertions;

    /// <summary>
    /// Тести для сервісу роботи з проєктами.
    /// </summary>
    public class ProjectServiceTests
    {
        private readonly Mock<ILogger<ProjectService>> _loggerMock;
        private readonly Mock<IProjectRepository> _repositoryMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _loggerMock = new Mock<ILogger<ProjectService>>();
            _repositoryMock = new Mock<IProjectRepository>();
            _service = new ProjectService(_loggerMock.Object, _repositoryMock.Object);
        }

        #region CreateProjectAsync

        /// <summary>
        /// Тест: створення проєкту з коректною назвою.
        /// Очікується успішний результат, повернення ідентифікатора проєкту та виклик репозиторію.
        /// </summary>
        [Fact]
        public async Task CreateProjectAsync_WithValidName_ReturnsSuccessAndProjectId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectName = "Мій тестовий проєкт";
            Project? createdProject = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>()))
                .Callback<Project>(p => createdProject = p)
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateProjectAsync(userId, projectName);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Проєкт успішно створено");
            result.ProjectId.Should().NotBeNull();
            createdProject.Should().NotBeNull();
            createdProject!.UserId.Should().Be(userId);
            createdProject.Name.Should().Be(projectName);
            createdProject.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            createdProject.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            createdProject.GraphData.Should().Be("{}");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        }

        /// <summary>
        /// Тест: передача null замість назви проєкту.
        /// Очікується використання назви за замовчуванням "Новий проєкт".
        /// </summary>
        [Fact]
        public async Task CreateProjectAsync_WithNullName_UsesDefaultName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Project? createdProject = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>()))
                .Callback<Project>(p => createdProject = p)
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateProjectAsync(userId, null);

            // Assert
            result.Success.Should().BeTrue();
            result.ProjectId.Should().NotBeNull();
            createdProject!.Name.Should().Be("Новий проєкт");
        }

        /// <summary>
        /// Тест: передача порожнього рядка або пробілів замість назви.
        /// Очікується використання назви за замовчуванням "Новий проєкт".
        /// </summary>
        [Fact]
        public async Task CreateProjectAsync_WithEmptyName_UsesDefaultName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Project? createdProject = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>()))
                .Callback<Project>(p => createdProject = p)
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateProjectAsync(userId, "   ");

            // Assert
            result.Success.Should().BeTrue();
            createdProject!.Name.Should().Be("Новий проєкт");
        }

        /// <summary>
        /// Тест: передача назви, що перевищує 100 символів.
        /// Очікується помилка, репозиторій не викликається.
        /// </summary>
        [Fact]
        public async Task CreateProjectAsync_WithTooLongName_ReturnsFailureAndDoesNotCallRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var longName = new string('a', 101);

            // Act
            var result = await _service.CreateProjectAsync(userId, longName);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Назва проєкту не може перевищувати 100 символів");
            result.ProjectId.Should().BeNull();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Never);
        }

        #endregion

        #region SaveProjectAsync

        /// <summary>
        /// Тест: спроба зберегти проєкт з назвою довшою за 100 символів.
        /// Очікується помилка, репозиторій не викликається.
        /// </summary>
        [Fact]
        public async Task SaveProjectAsync_WhenNameTooLong_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var longName = new string('b', 101);

            // Act
            var result = await _service.SaveProjectAsync(userId, projectId, longName, null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Назва проєкту не може перевищувати 100 символів");
            _repositoryMock.Verify(r => r.GetByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Never);
        }

        /// <summary>
        /// Тест: спроба зберегти неіснуючий проєкт.
        /// Очікується помилка "Проєкт не знайдено", оновлення не виконується.
        /// </summary>
        [Fact]
        public async Task SaveProjectAsync_WhenProjectNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(null as Project);

            // Act
            var result = await _service.SaveProjectAsync(userId, projectId, "New Name", null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Проєкт не знайдено");
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Never);
        }

        /// <summary>
        /// Тест: збереження лише нової назви для існуючого проєкту.
        /// Очікується оновлення назви та часу оновлення.
        /// </summary>
        [Fact]
        public async Task SaveProjectAsync_WithValidNameOnly_UpdatesNameAndTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Old Name",
                GraphData = "{}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            var newName = "Updated Name";
            var originalUpdatedAt = existingProject.UpdatedAt;

            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(existingProject);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SaveProjectAsync(userId, projectId, newName, null);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Проєкт успішно збережено");
            existingProject.Name.Should().Be(newName);
            existingProject.GraphData.Should().Be("{}");
            existingProject.UpdatedAt.Should().BeAfter(originalUpdatedAt);
            _repositoryMock.Verify(r => r.UpdateAsync(existingProject), Times.Once);
        }

        /// <summary>
        /// Тест: збереження лише даних графа (JSON) для існуючого проєкту.
        /// Очікується оновлення GraphData та часу оновлення.
        /// </summary>
        [Fact]
        public async Task SaveProjectAsync_WithValidGraphDataOnly_UpdatesGraphDataAndTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Some Project",
                GraphData = "{}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            var newGraphData = "{\"nodes\":[{\"id\":\"1\",\"label\":\"v1\"}]}";
            var originalUpdatedAt = existingProject.UpdatedAt;

            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(existingProject);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SaveProjectAsync(userId, projectId, null, newGraphData);

            // Assert
            result.Success.Should().BeTrue();
            existingProject.GraphData.Should().Be(newGraphData);
            existingProject.Name.Should().Be("Some Project");
            existingProject.UpdatedAt.Should().BeAfter(originalUpdatedAt);
            _repositoryMock.Verify(r => r.UpdateAsync(existingProject), Times.Once);
        }

        /// <summary>
        /// Тест: збереження одночасно і нової назви, і даних графа.
        /// Очікується оновлення обох полів.
        /// </summary>
        [Fact]
        public async Task SaveProjectAsync_WithBothNameAndGraphData_UpdatesBoth()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Old Name",
                GraphData = "{}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            var newName = "New Name";
            var newGraphData = "{\"edges\":[]}";

            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(existingProject);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SaveProjectAsync(userId, projectId, newName, newGraphData);

            // Assert
            result.Success.Should().BeTrue();
            existingProject.Name.Should().Be(newName);
            existingProject.GraphData.Should().Be(newGraphData);
            _repositoryMock.Verify(r => r.UpdateAsync(existingProject), Times.Once);
        }

        #endregion

        #region GetProjectsAsync

        /// <summary>
        /// Тест: отримання списку проєктів користувача, коли вони є.
        /// Очікується успіх та повернення списку.
        /// </summary>
        [Fact]
        public async Task GetProjectsAsync_ReturnsListFromRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fakeProjects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Name = "Proj1", UserId = userId },
                new Project { Id = Guid.NewGuid(), Name = "Proj2", UserId = userId }
            };
            _repositoryMock.Setup(r => r.GetProjectsByUserIdAsync(userId))
                .ReturnsAsync(fakeProjects);

            // Act
            var result = await _service.GetProjectsAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Проєкти успішно отримано");
            result.Projects.Should().BeEquivalentTo(fakeProjects);
            _repositoryMock.Verify(r => r.GetProjectsByUserIdAsync(userId), Times.Once);
        }

        /// <summary>
        /// Тест: отримання списку проєктів, коли у користувача немає жодного проєкту.
        /// Очікується успіх та порожній список.
        /// </summary>
        [Fact]
        public async Task GetProjectsAsync_WhenNoProjects_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetProjectsByUserIdAsync(userId))
                .ReturnsAsync(new List<Project>());

            // Act
            var result = await _service.GetProjectsAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Projects.Should().BeEmpty();
        }

        #endregion

        #region GetProjectAsync

        /// <summary>
        /// Тест: отримання конкретного проєкту за ідентифікатором, коли він існує.
        /// Очікується успіх та повернення проєкту.
        /// </summary>
        [Fact]
        public async Task GetProjectAsync_WhenProjectExists_ReturnsProject()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var expectedProject = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Test Project",
                GraphData = "{\"nodes\":[]}"
            };
            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(expectedProject);

            // Act
            var result = await _service.GetProjectAsync(userId, projectId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Проєкт успішно отримано");
            result.Project.Should().Be(expectedProject);
            _repositoryMock.Verify(r => r.GetByIdAndUserIdAsync(projectId, userId), Times.Once);
        }

        /// <summary>
        /// Тест: отримання неіснуючого проєкту.
        /// Очікується помилка "Проєкт не знайдено".
        /// </summary>
        [Fact]
        public async Task GetProjectAsync_WhenProjectNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(null as Project);

            // Act
            var result = await _service.GetProjectAsync(userId, projectId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Проєкт не знайдено");
            result.Project.Should().BeNull();
            _repositoryMock.Verify(r => r.GetByIdAndUserIdAsync(projectId, userId), Times.Once);
        }

        #endregion

        #region DeleteProjectAsync

        /// <summary>
        /// Тест: видалення існуючого проєкту.
        /// Очікується успіх, репозиторій викликає метод DeleteAsync.
        /// </summary>
        [Fact]
        public async Task DeleteProjectAsync_WhenProjectExists_DeletesAndReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "To Delete"
            };

            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(existingProject);
            _repositoryMock.Setup(r => r.DeleteAsync(existingProject))
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteProjectAsync(userId, projectId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Проєкт успішно видалено");
            _repositoryMock.Verify(r => r.DeleteAsync(existingProject), Times.Once);
        }

        /// <summary>
        /// Тест: спроба видалити неіснуючий проєкт.
        /// Очікується помилка "Проєкт не знайдено", метод DeleteAsync не викликається.
        /// </summary>
        [Fact]
        public async Task DeleteProjectAsync_WhenProjectNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAndUserIdAsync(projectId, userId))
                .ReturnsAsync(null as Project);

            // Act
            var result = await _service.DeleteProjectAsync(userId, projectId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Проєкт не знайдено");
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Project>()), Times.Never);
        }

        #endregion
    }
}