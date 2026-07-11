using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
