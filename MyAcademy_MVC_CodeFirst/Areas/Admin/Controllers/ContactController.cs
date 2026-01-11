using MyAcademy_MVC_CodeFirst.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        AppDbContext context = new AppDbContext();
        public ActionResult Index()
        {
            var values = context.Contacts.OrderByDescending(x => x.SendDate).ToList();
            return View(values);
        }
        public ActionResult MessageDetails(int id)
        {
            var value = context.Contacts.Find(id);
            if (value != null)
            {
                value.IsRead = true;
                context.SaveChanges();
            }
            return View(value);
        }
        public ActionResult DeleteContact(int id)
        {
            var value = context.Contacts.Find(id);
            context.Contacts.Remove(value);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}