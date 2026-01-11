using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PackagesController : Controller
    {
        AppDbContext context = new AppDbContext();
        public ActionResult Index()
        {
            var values = context.Packages.ToList();
            return View(values);
        }

        [HttpGet]
        public ActionResult CreatePackage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePackage(Packages package)
        {
            context.Packages.Add(package);
            context.SaveChanges();
            TempData["Success"] = "Yeni paket başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UpdatePackage(int id)
        {
            var value = context.Packages.Find(id);
            return View(value);
        }

        [HttpPost]
        public ActionResult UpdatePackage(Packages package)
        {
            var value = context.Packages.Find(package.Id);

            if (value != null)
            {
                value.Name = package.Name;
                value.Price = package.Price;
                value.Item1 = package.Item1;
                value.Item2 = package.Item2;
                value.Item3 = package.Item3;
                value.Item4 = package.Item4;

                context.SaveChanges();

                TempData["Success"] = "Paket bilgileri başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Paket bulunamadı!";
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeletePackage(int id)
        {
            var value = context.Packages.Find(id);
            context.Packages.Remove(value);
            context.SaveChanges();
            TempData["Success"] = "Paket başarıyla silindi.";
            return RedirectToAction("Index");
        }

    }
}