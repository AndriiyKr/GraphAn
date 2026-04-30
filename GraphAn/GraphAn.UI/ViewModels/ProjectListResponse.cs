// <copyright file="ProjectListResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Відповідь зі списком проектів користувача.
    /// </summary>
    public class ProjectListResponse
    {
        /// <summary>
        /// Отримує або задає список проектів.
        /// </summary>
        public List<ProjectShortResponse> Projects { get; set; } = new List<ProjectShortResponse>();
    }
}