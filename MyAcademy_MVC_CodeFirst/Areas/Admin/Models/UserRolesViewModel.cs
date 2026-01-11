using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Collections.Generic;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();
        public List<AppRole> AllRoles { get; set; } = new List<AppRole>();
    }
}