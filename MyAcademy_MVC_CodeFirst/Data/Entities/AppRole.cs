using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class AppRole : IdentityRole<string, IdentityUserRole>
    {
        public AppRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public AppRole(string name) : this()
        {
            Name = name;
        }
    }
}