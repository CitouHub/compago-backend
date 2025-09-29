using System;
using System.Collections.Generic;

namespace Compago.Data;

public partial class InvoiceTag
{
    public string InvoiceId { get; set; } = null!;

    public short TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Tag Tag { get; set; } = null!;
}
