using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyAcademy_MVC_CodeFirst.Data.Context;

namespace MyAcademy_MVC_CodeFirst.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    public class PolicyUserController : Controller
    {
        private AppDbContext context = new AppDbContext();

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            string currentUserId = User.Identity.GetUserId();

            var policies = context.Policies
                .Include(x => x.Category)
                .Where(x => x.AppUserId == currentUserId)
                .OrderByDescending(x => x.PolicyNumber)
                .ToList();

            if (policies.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Giriş Yapan ID: " + currentUserId);
            }

            return View(policies);
        }

        public ActionResult Details(int id)
        {
            var currentUserId = User.Identity.GetUserId();

            var policy = context.Policies
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == id && x.AppUserId == currentUserId);

            if (policy == null)
            {
                return HttpNotFound("Poliçe bulunamadı veya bu poliçeye erişim yetkiniz yok.");
            }

            return View(policy);
        }
    }
}
