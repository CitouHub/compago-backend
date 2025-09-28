using System;
using System.Collections.Generic;

namespace Compago.Data;

public partial class User
{
    public int Id { get; set; }

    public long CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public DateTime? UpdatedBy { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
