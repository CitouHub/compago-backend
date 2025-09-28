namespace Compago.Domain.ExternalSourceExample.MicosoftAzure
{
    public class Monthly
    {
        public DateTime IssueDate { get; set; }
        public Bill Bill { get; set; } = null!;
    }
}
