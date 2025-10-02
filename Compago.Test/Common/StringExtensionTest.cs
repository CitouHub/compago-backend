using Compago.Common.Extension;

namespace Compago.Test.Common
{
    public class StringExtensionTest
    {
        [Theory]
        [InlineData("#000000", true)]
        [InlineData("#FFFFFF", true)]
        [InlineData("#ABCDEF", true)]
        [InlineData("#123456", true)]
        [InlineData("#7890AB", true)]
        [InlineData("#0000000", false)]
        [InlineData("#00000", false)]
        [InlineData("##00000", false)]
        [InlineData("#G00000", false)]
        [InlineData("#0G0000", false)]
        [InlineData("#00G000", false)]
        [InlineData("#000G00", false)]
        [InlineData("#0000G0", false)]
        [InlineData("#00000G", false)]
        public void IsColorCode(string code, bool expexted)
        {
            // Arrage
            var upperCode = code.ToUpper();
            var lowerCode = code.ToLower();

            // Act
            var upperIsColor = upperCode.IsColorCode();
            var lowerIsColor = lowerCode.IsColorCode();

            // Assert
            Assert.Equal(expexted, upperIsColor);
            Assert.Equal(expexted, lowerIsColor);
        }
    }
}
