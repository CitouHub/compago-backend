namespace Compago.Test.Service
{
    public class CacheSerivceTest : ServiceTest
    {
        [Fact]
        public void DoNotExists()
        {
            // Arrange
            var cacheService = GetCacheService();

            // Act
            var result = cacheService.Get<int>();

            // Assert
            Assert.Equal(default, result);
        }
    }
}
