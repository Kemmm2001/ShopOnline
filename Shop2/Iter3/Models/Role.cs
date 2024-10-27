﻿using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRolesAuditNewRoles = new HashSet<UserRolesAudit>();
            UserRolesAuditOldRoles = new HashSet<UserRolesAudit>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<UserRolesAudit> UserRolesAuditNewRoles { get; set; }
        public virtual ICollection<UserRolesAudit> UserRolesAuditOldRoles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}