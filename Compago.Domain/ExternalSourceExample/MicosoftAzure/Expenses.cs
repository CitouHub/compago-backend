namespace Compago.Domain.ExternalSourceExample.MicosoftAzure
{
    public class Expenses
    {
        public string Currency { get; set; } = null!;
        public List<Monthly> Monthly { get; set; } = null!;
    }
}
