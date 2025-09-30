using Compago.Common;

namespace Compago.Test.Common
{
    public class PasswordHandlerTest
    {
        [Theory]
        [InlineData("abc123", "abc123", true)]
        [InlineData("abc123", "abc1234", false)]
        [InlineData("abc123", "Abc123", false)]
        public void ValidatePassword(string userPassword, string loginPassword, bool expectedEqual)
        {
            // Arrange
            var hash = PasswordHandler.HashPassword(userPassword);

            // Act
            var equal = PasswordHandler.ValidatePassword(loginPassword, hash.hash, hash.salt);

            // Assert
            Assert.Equal(expectedEqual, equal);
        }
    }
}
