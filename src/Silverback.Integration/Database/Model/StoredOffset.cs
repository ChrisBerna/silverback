﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.ComponentModel.DataAnnotations;

namespace Silverback.Database.Model
{
    /// <summary>
    ///     The entity stored in the offset storage table.
    /// </summary>
    public class StoredOffset
    {
        /// <summary>
        ///     Gets or sets the offset key.
        /// </summary>
        [Key]
        [MaxLength(300)]
        public string Key { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the CLR type of the stored offset.
        /// </summary>
        [MaxLength(300)]
        public string? ClrType { get; set; }

        /// <summary>
        ///     Gets or sets the offset value.
        /// </summary>
        [MaxLength(300)]
        public string? Value { get; set; }

        /// <summary>
        ///     Gets or sets the serialized offset.
        /// </summary>
        [MaxLength(500)]
        [Obsolete("Now replaced by ClrType and Value.")]
        public string? Offset { get; set; }
    }
}
