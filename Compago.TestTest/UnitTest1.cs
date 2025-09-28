using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Compago.TestTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var application = new WebApplicationFactory<Compago.TestTest>();
            var c = application.CreateClient();
            Assert.Equal(1, 1);
        }
    }
}
