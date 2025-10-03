using Compago.Common;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class UserSecurityCredentialsHelper
    {
        public static UserSecurityCredentialsDTO New(
            int id = 1,
            Role roleId = Role.Admin,
            string passwordHash = "passwordHash",
            string passwordHashSalt = "PasswordHashSalt")
        {
            return new UserSecurityCredentialsDTO()
            {
                Id = id,
                RoleId = roleId,
                PasswordHash = passwordHash,
                PasswordHashSalt = passwordHashSalt
            };
        }
    }
}
