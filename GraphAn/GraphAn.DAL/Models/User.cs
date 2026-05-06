// <copyright file="User.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Представляє користувача системи.
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.Projects = new HashSet<Project>();
        }

        /// <summary>
        /// Отримує або задає дату створення акаунту.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Отримує або задає список проектів, що належать користувачу.
        /// </summary>
        public virtual ICollection<Project> Projects { get; set; }
    }
}