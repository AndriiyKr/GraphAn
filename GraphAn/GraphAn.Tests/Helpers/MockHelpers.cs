// <copyright file="MockHelpers.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Tests.Helpers
{
    using GraphAn.DAL.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using System.Threading.Tasks;

    /// <summary>
    /// Допоміжні методи для створення моків Identity компонентів.
    /// </summary>
    public static class MockHelpers
    {
        /// <summary>
        /// Створює мок <see cref="UserManager{TUser}"/> для типу <see cref="User"/>.
        /// </summary>
        /// <returns>Мок об'єкта <see cref="UserManager{User}"/> з налаштованими заглушками.</returns>
        public static Mock<UserManager<User>> MockUserManager()
        {
            // Створюємо необхідні залежності
            var store = new Mock<IUserStore<User>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<User>>();
            var userValidators = new IUserValidator<User>[] { };
            var passwordValidators = new IPasswordValidator<User>[] { };
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<User>>>();

            var mgr = new Mock<UserManager<User>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);

            return mgr;
        }

        /// <summary>
        /// Створює мок <see cref="SignInManager{TUser}"/> для типу <see cref="User"/>.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <returns>Мок об'єкта <see cref="SignInManager{User}"/>.</returns>
        public static Mock<SignInManager<User>> MockSignInManager(Mock<UserManager<User>> userManagerMock)
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<User>>>();

            var authSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmation = new Mock<IUserConfirmation<User>>();

            var signInManager = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                authSchemeProvider.Object,
                userConfirmation.Object);

            return signInManager;
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод FindByEmailAsync повертав користувача.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="email">Email, за яким буде виконано пошук.</param>
        /// <param name="user">Користувач, який буде повернений (може бути null).</param>
        public static void SetupFindByEmailAsync(this Mock<UserManager<User>> userManagerMock, string email, User? user)
        {
            userManagerMock
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод FindByNameAsync повертав користувача.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="userName">Ім'я користувача, за яким буде виконано пошук.</param>
        /// <param name="user">Користувач, який буде повернений (може бути null).</param>
        public static void SetupFindByNameAsync(this Mock<UserManager<User>> userManagerMock, string userName, User? user)
        {
            userManagerMock
                .Setup(x => x.FindByNameAsync(userName))
                .ReturnsAsync(user);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод CreateAsync повертав успішний результат.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="expectedUser">Очікуваний об'єкт користувача при створенні (опціонально).</param>
        public static void SetupCreateAsyncSuccess(this Mock<UserManager<User>> userManagerMock, User? expectedUser = null)
        {
            userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((user, _) =>
                {
                    if (expectedUser != null)
                    {
                        expectedUser.Id = user.Id;
                        expectedUser.UserName = user.UserName;
                        expectedUser.Email = user.Email;
                    }
                });
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод CreateAsync повертав помилку.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="errorMessage">Повідомлення помилки.</param>
        public static void SetupCreateAsyncFailure(this Mock<UserManager<User>> userManagerMock, string errorMessage)
        {
            var identityError = new IdentityError { Description = errorMessage };
            var result = IdentityResult.Failed(identityError);
            userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод GenerateEmailConfirmationTokenAsync повертав фіктивний токен.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="returnToken">Токен, який буде повернуто.</param>
        public static void SetupGenerateEmailConfirmationTokenAsync(this Mock<UserManager<User>> userManagerMock, string returnToken = "test-token")
        {
            userManagerMock
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(returnToken);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод ConfirmEmailAsync повертав успішний результат.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        public static void SetupConfirmEmailAsyncSuccess(this Mock<UserManager<User>> userManagerMock)
        {
            userManagerMock
                .Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод IsEmailConfirmedAsync повертав задане значення.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="isConfirmed">Значення, яке буде повернено.</param>
        public static void SetupIsEmailConfirmedAsync(this Mock<UserManager<User>> userManagerMock, bool isConfirmed)
        {
            userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(It.IsAny<User>()))
                .ReturnsAsync(isConfirmed);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод GeneratePasswordResetTokenAsync повертав фіктивний токен.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="returnToken">Токен, який буде повернуто.</param>
        public static void SetupGeneratePasswordResetTokenAsync(this Mock<UserManager<User>> userManagerMock, string returnToken = "reset-token")
        {
            userManagerMock
                .Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(returnToken);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод ResetPasswordAsync повертав успішний результат.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        public static void SetupResetPasswordAsyncSuccess(this Mock<UserManager<User>> userManagerMock)
        {
            userManagerMock
                .Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
        }

        /// <summary>
        /// Налаштовує мок <see cref="UserManager{TUser}"/> так, щоб метод ResetPasswordAsync повертав помилку.
        /// </summary>
        /// <param name="userManagerMock">Мок <see cref="UserManager{User}"/>.</param>
        /// <param name="errorMessage">Повідомлення помилки.</param>
        public static void SetupResetPasswordAsyncFailure(this Mock<UserManager<User>> userManagerMock, string errorMessage)
        {
            var identityError = new IdentityError { Description = errorMessage };
            var result = IdentityResult.Failed(identityError);
            userManagerMock
                .Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        /// <summary>
        /// Налаштовує мок <see cref="SignInManager{TUser}"/> так, щоб метод PasswordSignInAsync повертав успішний результат.
        /// </summary>
        /// <param name="signInManagerMock">Мок <see cref="SignInManager{User}"/>.</param>
        public static void SetupPasswordSignInSuccess(this Mock<SignInManager<User>> signInManagerMock)
        {
            signInManagerMock
                .Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);
        }

        /// <summary>
        /// Налаштовує мок <see cref="SignInManager{TUser}"/> так, щоб метод PasswordSignInAsync повертав заданий результат.
        /// </summary>
        /// <param name="signInManagerMock">Мок <see cref="SignInManager{User}"/>.</param>
        /// <param name="result">Результат входу (наприклад, SignInResult.Failed).</param>
        public static void SetupPasswordSignInResult(this Mock<SignInManager<User>> signInManagerMock, SignInResult result)
        {
            signInManagerMock
                .Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(result);
        }

        /// <summary>
        /// Налаштовує мок <see cref="SignInManager{TUser}"/> так, щоб метод SignInAsync виконувався без помилок.
        /// </summary>
        /// <param name="signInManagerMock">Мок <see cref="SignInManager{User}"/>.</param>
        public static void SetupSignInAsync(this Mock<SignInManager<User>> signInManagerMock)
        {
            signInManagerMock
                .Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .Returns(Task.CompletedTask);
        }
    }
}