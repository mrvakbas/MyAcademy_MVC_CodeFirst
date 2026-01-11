using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        AppDbContext context = new AppDbContext();
        public ActionResult Index()
        {
            var categories = context.Categories.ToList();
            return View(categories);
        }
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCategory(int id)
        {
            var category = context.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public ActionResult UpdateCategory(Category model)
        {
            var category = context.Categories.Find(model.Id);
            category.Name = model.Name;
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteCategory(int id)
        {
            var category = context.Categories.Find(id);
            context.Categories.Remove(category);    
            context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}