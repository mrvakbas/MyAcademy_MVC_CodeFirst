using System;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class Contact
    {
        public int ContactID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string InsuranceType { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false; 
    }
}