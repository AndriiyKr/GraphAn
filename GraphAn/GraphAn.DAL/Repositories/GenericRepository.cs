// <copyright file="GenericRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    using GraphAn.DAL.Context;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a generic repository that provides basic CRUD operations for entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class GenericRepository<T>
        where T : class
    {
        /// <summary>
        /// The database context used for data access.
        /// </summary>
        private readonly AppDbContext context;

        /// <summary>
        /// The database set for the entity type.
        /// </summary>
        private readonly DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations. Cannot be <see langword="null"/>.</param>
        public GenericRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<T>();
        }

        /// <summary>
        /// Gets the database context for derived classes.
        /// </summary>
        protected AppDbContext Context => this.context;

        /// <summary>
        /// Gets the database set for derived classes.
        /// </summary>
        protected DbSet<T> DbSet => this.dbSet;

        /// <summary>
        /// Retrieves all entities from the data source.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of all entities.
        /// </returns>
        public async Task<List<T>> GetAllAsync()
        {
            return await this.dbSet.ToListAsync();
        }

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the entity if found; otherwise, <see langword="null"/>.
        /// </returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await this.dbSet.FindAsync(id);
        }

        /// <summary>
        /// Adds a new entity to the data source.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the number of state entries written to the database.
        /// </returns>
        public async Task<int> AddAsync(T entity)
        {
            await this.dbSet.AddAsync(entity);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing entity in the data source.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the number of state entries written to the database.
        /// </returns>
        public async Task<int> UpdateAsync(T entity)
        {
            this.dbSet.Update(entity);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity from the data source.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the number of state entries written to the database.
        /// </returns>
        public async Task<int> DeleteAsync(T entity)
        {
            this.dbSet.Remove(entity);
            return await this.context.SaveChangesAsync();
        }
    }
}