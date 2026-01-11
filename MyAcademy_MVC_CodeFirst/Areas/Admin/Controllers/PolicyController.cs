using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PolicyController : Controller
    {
        AppDbContext context = new AppDbContext();

        public ActionResult Index()
        {
            var values = context.Policies
                .Include(p => p.Category)
                .Include(p => p.AppUser)
                .ToList();
            return View(values);
        }

        [HttpGet]
        public ActionResult CreatePolicy()
        {
            PrepareViewBags();

            var lastId = context.Policies.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
            ViewBag.NextPolicyNumber = "POL-" + (lastId + 1001);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePolicy(Policy policy)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.Name;
                    var currentUser = context.Users.FirstOrDefault(x => x.UserName == userName);

                    if (currentUser != null)
                    {
                        policy.City = currentUser.City;

                        var lastId = context.Policies.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                        policy.PolicyNumber = "POL-" + (lastId + 1001);
                    }

                    policy.IsActive = true; 

                    context.Policies.Add(policy);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                }
            }

            PrepareViewBags();
            return View(policy);
        }
        private void PrepareViewBags()
        {
            ViewBag.CategoryId = new SelectList(context.Categories.ToList(), "Id", "Name");

            var userRoleId = context.Roles.FirstOrDefault(r => r.Name == "User")?.Id;
            var customers = context.Users
                .Where(u => u.Roles.Any(r => r.RoleId == userRoleId))
                .ToList()
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = $"{u.Name} {u.Surname} - {u.UserName}"
                });

            ViewBag.AppUserId = new SelectList(customers, "Id", "FullName");

            var currentUser = context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            ViewBag.UserCity = currentUser?.City ?? "Merkez Şube";
            var lastId = context.Policies.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
            ViewBag.NextPolicyNumber = "POL-" + (lastId + 1001);
        }

        [HttpGet]
        public ActionResult UpdatePolicy(int id)
        {
            var policy = context.Policies.Find(id);

            ViewBag.CategoryId = new SelectList(context.Categories.ToList(), "Id", "Name", policy.CategoryId);

            var userRoleId = context.Roles.FirstOrDefault(r => r.Name == "User")?.Id;
            var users = context.Users
                .Where(u => u.Roles.Any(r => r.RoleId == userRoleId))
                .ToList()
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = $"{u.Name} {u.Surname} - {u.UserName}"
                });

            ViewBag.AppUserId = new SelectList(users, "Id", "FullName", policy.AppUserId);

            return View(policy);
        }

        [HttpPost]
        public ActionResult UpdatePolicy(Policy policy)
        {
            var value = context.Policies.Find(policy.Id);

            bool oldStatus = value.IsActive;

            value.PolicyNumber = policy.PolicyNumber;
            value.Price = policy.Price;
            value.StartDate = policy.StartDate;
            value.EndDate = policy.EndDate;
            value.CategoryId = policy.CategoryId;
            value.AppUserId = policy.AppUserId;
            value.IsActive = policy.IsActive;

            context.SaveChanges();

            if (oldStatus != policy.IsActive)
            {
                var adminUserName = User.Identity.Name;
                var admin = context.Users.FirstOrDefault(x => x.UserName == adminUserName);

                if (admin != null)
                {
                    string statusText = policy.IsActive ? "aktif hale getirildi" : "pasif hale getirildi";

                    context.LoginLogs.Add(new LoginLog
                    {
                        LogDate = DateTime.Now,
                        Message = $"{admin.Name} {admin.Surname} ({admin.UserName}) {value.PolicyNumber} numaralı poliçeyi {statusText}"
                    });

                    context.SaveChanges();
                }
            }

            TempData["SuccessMessage"] = "Poliçe başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

    }
}