using Compago.Common;

namespace Compago.Domain
{
    public class UserSecurityCredentialsDTO
    {
        public int Id { get; set; }

        public Role RoleId { get; set; }

        public string PasswordHash { get; set; } = null!;

        public string PasswordHashSalt { get; set; } = null!;
    }
}
