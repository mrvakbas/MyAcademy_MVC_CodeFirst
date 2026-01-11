using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        AppDbContext context = new AppDbContext();


        private void GetCategories()
        {
            var categories = context.Categories.ToList();
            ViewBag.categories = (from category in categories
                                  select new SelectListItem
                                  {
                                      Text = category.Name,
                                      Value = category.Id.ToString(),
                                  }).ToList();
        }

        public ActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }

        public ActionResult CreateProduct()
        {
            GetCategories();
            return View();
        }
        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public ActionResult UpdateProduct(int id)
        {
            GetCategories();
            var product = context.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public ActionResult UpdateProduct(Product model)
        {
            var product = context.Products.Find(model.Id);
            product.Name = model.Name;
            product.Description = model.Description;
            product.ImageUrl = model.ImageUrl;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult DeleteProduct(int id)
        {
            var product = context.Products.Find(id);
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}