using Compago.Data;

namespace Compago.Test.Helper
{
    public static class RoleHelper
    {
        public static Role NewDb(
            short id = 1,
            string name = "TestRole")
        {
            return new Role()
            {
                Id = id,
                Name = name,
            };
        }
    }
}
