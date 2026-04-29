// <copyright file="CodeConfirmRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    public class CodeConfirmRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
