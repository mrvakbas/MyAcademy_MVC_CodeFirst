using System.ComponentModel.DataAnnotations;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}