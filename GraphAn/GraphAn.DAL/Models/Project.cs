// <copyright file="Project.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє проект з даними графа.
    /// </summary>
    [Table("projects")]
    public class Project
    {
        /// <summary>
        /// Отримує або задає унікальний ідентифікатор проекту.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Отримує або задає ідентифікатор власника проекту.
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Отримує або задає назву проекту.
        /// </summary>
        [Required]
        [Column("name")]
        required public string Name { get; set; }

        /// <summary>
        /// Отримує або задає дані графа у форматі JSON.
        /// </summary>
        [Column("graph_data", TypeName = "jsonb")]
        public string? GraphData { get; set; }

        /// <summary>
        /// Отримує або задає час створення.
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Отримує або задає час останнього оновлення.
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Отримує або задає навігаційну властивість до користувача.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}