// <copyright file="Registration.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.DAL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє тимчасову транзакцію реєстрації користувача.
    /// </summary>
    [Table("registrations")]
    public class Registration
    {
        /// <summary>
        /// Отримує або задає ідентифікатор транзакції.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Отримує або задає електронну пошту для реєстрації.
        /// </summary>
        [Required]
        [Column("email")]
        required public string Email { get; set; }

        /// <summary>
        /// Отримує або задає код верифікації.
        /// </summary>
        [Required]
        [Column("verification_code")]
        required public string VerificationCode { get; set; }

        /// <summary>
        /// Отримує або задає час закінчення дії коду.
        /// </summary>
        [Column("code_expiry_time")]
        public DateTime CodeExpiryTime { get; set; }

        /// <summary>
        /// Отримує або задає тимчасово збережений пароль.
        /// </summary>
        [Required]
        [Column("temp_password_hash")]
        required public string TempPasswordHash { get; set; }

        /// <summary>
        /// Отримує або задає тимчасовий нікнейм.
        /// </summary>
        [Required]
        [Column("temp_username")]
        required public string TempUsername { get; set; }
    }
}