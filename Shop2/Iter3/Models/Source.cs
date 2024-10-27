using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class Source
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int FeedbackId { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
    }
}
