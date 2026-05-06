// <copyright file="ProjectViewModel.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    using System;

    /// <summary>
    /// Модель представлення для сторінки редактора проєкту.
    /// </summary>
    public class ProjectViewModel
    {
        /// <summary>
        /// Отримує або задає унікальний ідентифікатор проєкту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Отримує або задає назву проєкту.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає дані графа у форматі JSON.
        /// </summary>
        public string? GraphData { get; set; }
    }
}