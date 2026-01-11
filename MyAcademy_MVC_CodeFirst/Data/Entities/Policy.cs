using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}