using System;
using System.Collections.Generic;

namespace Iter3.Models
{
    public partial class BlogCategory
    {
        public BlogCategory()
        {
            Blogs = new HashSet<Blog>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
