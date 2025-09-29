using System;
using System.Collections.Generic;

namespace Compago.Data;

public partial class Tag
{
    public short Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<InvoiceTag> InvoiceTags { get; set; } = new List<InvoiceTag>();
}
