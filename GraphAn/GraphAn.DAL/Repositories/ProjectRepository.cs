// <copyright file="ProjectRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Context;
    using GraphAn.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a Project repository.
    /// </summary>
    public class ProjectRepository : GenericRepository<Project>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations. Cannot be <see langword="null"/>.</param>
        public ProjectRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}