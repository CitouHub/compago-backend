using Compago.Data;

namespace Compago.Domain;

public class TagDTO
{
    public short? Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; } = null!;

    public List<InvoiceTag> InvoiceTags { get; set; } = [];
}
