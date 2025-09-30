using Compago.Data;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class UserHelper
    {
        public static UserDTO New(
            int? id = 1,
            string username = "TestUser",
            string? password = "Password",
            Compago.Common.Role roleId = Compago.Common.Role.Admin)
        {
            return new UserDTO()
            {
                Id = id,
                Username = username,
                Password = password,
                RoleId = roleId
            };
        }

        public static User NewDb(
            int id = 1,
            string username = "TestUser",
            string passwordHash = "PasswordHash",
            string passwordHashSalt = "PasswordHashSalt",
            Compago.Common.Role roleId = Compago.Common.Role.Admin)
        {
            return new User()
            {
                Id = id,
                Username = username,
                PasswordHash = passwordHash,
                PasswordHashSalt = passwordHashSalt,
                RoleId = (short)roleId
            };
        }
    }
}
