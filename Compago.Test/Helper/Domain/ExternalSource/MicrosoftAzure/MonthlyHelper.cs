using Compago.Domain.ExternalSourceExample.MicosoftAzure;

namespace Compago.Test.Helper.Domain.ExternalSource.MicrosoftAzure
{
    public static class MonthlyHelper
    {
        public static Monthly New(DateTime? issueDate = null)
        {
            return new Monthly()
            {
                IssueDate = issueDate ?? DateTime.UtcNow
            };
        }
    }
}
