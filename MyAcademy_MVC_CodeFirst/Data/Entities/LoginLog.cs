using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAcademy_MVC_CodeFirst.Data.Entities
{
    public class LoginLog
    {
        public int Id { get; set; }

        public DateTime LogDate { get; set; }
        public string Message { get; set; }
    }
}