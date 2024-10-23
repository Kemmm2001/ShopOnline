using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int ProductSizePricingId { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ProductSizePricing ProductSizePricing { get; set; } = null!;
    }
}
