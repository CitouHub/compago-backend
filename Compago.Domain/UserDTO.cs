using Compago.Common;

namespace Compago.Domain;

public class UserDTO
{
    public int? Id { get; set; }

    public Role RoleId { get; set; }

    public string? RoleName { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }
}
