// <copyright file="User.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє користувача системи.
    /// </summary>
    [Table("users")]
    public class User
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="User"/>.
        /// </summary>
        public User()
        {
            this.Projects = new HashSet<Project>();
        }

        /// <summary>
        /// Отримує або задає унікальний ідентифікатор користувача.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Отримує або задає ім'я користувача.
        /// </summary>
        [Required]
        [Column("username")]
        public string Username { get; set; }

        /// <summary>
        /// Отримує або задає електронну пошту (унікальну).
        /// </summary>
        [Required]
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Отримує або задає хешований пароль.
        /// </summary>
        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Отримує або задає дату створення акаунту.
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Отримує або задає список проектів, що належать користувачу.
        /// </summary>
        public virtual ICollection<Project> Projects { get; set; }
    }
}