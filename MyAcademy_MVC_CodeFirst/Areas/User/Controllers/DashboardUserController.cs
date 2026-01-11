using Microsoft.AspNet.Identity;
using MyAcademy_MVC_CodeFirst.Data.Context;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    public class DashboardUserController : Controller
    {
        public ActionResult Index()
        {
            AppDbContext context = new AppDbContext();

            var currentUserId = User.Identity.GetUserId();

            var userPolicies = context.Policies.Where(x => x.AppUserId == currentUserId);

            ViewBag.TotalPolicies = userPolicies.Count();
            ViewBag.TotalRevenue = userPolicies.Sum(x => (decimal?)x.Price) ?? 0;
            ViewBag.ActivePolicies = userPolicies.Count(x => x.IsActive == true);
            ViewBag.ExpiredPolicies = userPolicies.Count(x => x.IsActive == false);

            var lastPolicies = userPolicies
                                 .Include(p => p.AppUser)
                                 .Include(x => x.Category)
                                 .OrderByDescending(x => x.PolicyNumber)
                                 .Take(5)
                                 .ToList();

            return View(lastPolicies);
        }
    }
}