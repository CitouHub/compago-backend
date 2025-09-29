using System;
using System.Collections.Generic;

namespace Compago.Data;

public partial class User
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public short RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
