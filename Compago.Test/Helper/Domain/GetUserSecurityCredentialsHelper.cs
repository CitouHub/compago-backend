using Compago.Common;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class GetUserSecurityCredentialsHelper
    {
        public static UserSecurityCredentialsDTO New(
                Role roleId = Role.Admin,
                string passwordHash = "passwordHash",
                string passwordHashSalt = "PasswordHashSalt"
            )
        {
            return new UserSecurityCredentialsDTO()
            {
                RoleId = roleId,
                PasswordHash = passwordHash,
                PasswordHashSalt = passwordHashSalt
            };
        }
    }
}
