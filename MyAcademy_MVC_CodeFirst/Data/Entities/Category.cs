using System.Collections.Generic;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<Product> Products { get; set; }
        public virtual ICollection<Policy> Policies { get; set; }
    }
}