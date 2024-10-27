﻿using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class Origin
    {
        public Origin()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CountryCode { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}