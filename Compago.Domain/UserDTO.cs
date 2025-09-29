namespace Compago.Domain;

public class UserDTO
{
    public int? Id { get; set; }

    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }
}
