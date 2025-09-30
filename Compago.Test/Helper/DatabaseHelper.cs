using Compago.Data;
using Microsoft.EntityFrameworkCore;

namespace Compago.Test.Helper
{
    public static class DatabaseHelper
    {
        public static async Task<CompagoDbContext> GetContextAsync()
        {
            var options = new DbContextOptionsBuilder<CompagoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbContext = new CompagoDbContext(options);

            return await PopluateDefaultsAsync(dbContext);
        }

        private static async Task<CompagoDbContext> PopluateDefaultsAsync(CompagoDbContext dbContext)
        {
            await dbContext.Roles.AddRangeAsync(Enum.GetValues<Compago.Common.Role>()
                .Where(_ => _ != 0)
                .Select(_ => new Role()
                {
                    Id = (short)_,
                    Name = _.ToString()
                }).ToList());

            await dbContext.SaveChangesAsync();

            return dbContext;
        }
    }
}
