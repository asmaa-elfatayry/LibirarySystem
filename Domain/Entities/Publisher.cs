using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Publisher:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactInfo { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
