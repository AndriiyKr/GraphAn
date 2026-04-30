// <copyright file="CreateProjectRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для створення нового проекту.
    /// </summary>
    public class CreateProjectRequest
    {
        /// <summary>
        /// Отримує або задає назву проекту.
        /// </summary>
        public string? Name { get; set; }
    }
}