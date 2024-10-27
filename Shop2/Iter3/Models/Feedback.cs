using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            Sources = new HashSet<Source>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
        public string Content { get; set; } = null!;
        public int Rating { get; set; }
        public bool Status { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Source> Sources { get; set; }
    }
}
