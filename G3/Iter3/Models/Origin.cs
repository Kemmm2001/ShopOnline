using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class Origin
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
    }
}
