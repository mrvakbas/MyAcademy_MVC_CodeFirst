using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Models
{
    public class UserCreateViewModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}