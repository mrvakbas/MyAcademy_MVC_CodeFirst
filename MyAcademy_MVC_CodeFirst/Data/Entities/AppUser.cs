using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class AppUser : IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public AppUser()
        {
        }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public virtual ICollection<Policy> Policies { get; set; }
    }
}