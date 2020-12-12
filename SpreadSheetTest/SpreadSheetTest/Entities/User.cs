using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SpreadSheetTest.Entities
{
    /// <summary>
    /// Describes a single user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// A unique ID to identify the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// A unique user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The hashed password of the user.
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Tables that this user owns.
        /// </summary>
        public ICollection<TableRef> OwnTables { get; set; }

        /// <summary>
        /// Tables that this user may edit.
        /// </summary>
        public ICollection<TableRef> Collaborations { get; set; }

        /// <summary>
        /// The role of this user.
        /// </summary>
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// Lists different roles users can have.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRole
    {
        /// <summary>
        /// A regular user with no special permissions.
        /// </summary>
        Common,

        /// <summary>
        /// A trusted user, basically an admin with readonly rights.
        /// Users of this kind may view any user's table.
        /// </summary>
        Manager,

        /// <summary>
        /// An administrator.
        /// Users of this kind may edit any user's tables.
        /// </summary>
        Admin
    }
}
