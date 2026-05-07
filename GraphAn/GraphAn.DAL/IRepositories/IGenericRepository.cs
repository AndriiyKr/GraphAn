// <copyright file="IGenericRepository.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Repositories
{
    /// <summary>
    /// Базовий інтерфейс репозиторію для CRUD операцій.
    /// </summary>
    /// <typeparam name="T">Тип сутності.</typeparam>
    public interface IGenericRepository<T>
        where T : class
    {
        /// <summary>
        /// Отримує всі сутності.
        /// </summary>
        /// <returns>Список всіх сутностей типу <typeparamref name="T"/>.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Отримує сутність за ідентифікатором.
        /// </summary>
        /// <param name="id">Ідентифікатор сутності.</param>
        /// <returns>Сутність типу <typeparamref name="T"/>, або <see langword="null"/>, якщо не знайдено.</returns>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Додає нову сутність.
        /// </summary>
        /// <param name="entity">Сутність для додавання.</param>
        /// <returns>Кількість записів, змінених у базі даних (зазвичай 1).</returns>
        Task<int> AddAsync(T entity);

        /// <summary>
        /// Оновлює існуючу сутність.
        /// </summary>
        /// <param name="entity">Сутність з оновленими даними.</param>
        /// <returns>Кількість записів, змінених у базі даних (зазвичай 1).</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Видаляє сутність.
        /// </summary>
        /// <param name="entity">Сутність для видалення.</param>
        /// <returns>Кількість записів, змінених у базі даних (зазвичай 1).</returns>
        Task<int> DeleteAsync(T entity);
    }
}