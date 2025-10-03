namespace Compago.Domain;

public class InvoiceTagDTO
{
    public string InvoiceId { get; set; } = null!;

    public short TagId { get; set; }

    public string? TagName { get; set; }

    public string? TagColor { get; set; }
}
