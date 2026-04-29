// <copyright file="LoginResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
