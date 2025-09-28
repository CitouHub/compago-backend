namespace Compago.Domain.ExternalSourceExample.MicosoftAzure
{
    public class Bill
    {
        public long Reference { get; set; }
        public string MoneyToPay { get; set; } = null!;
    }
}
