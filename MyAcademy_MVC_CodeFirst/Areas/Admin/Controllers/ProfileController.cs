using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAcademy_MVC_CodeFirst.Areas.Admin.Models;
using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private readonly RoleManager<AppRole, string> roleManager;
        private readonly UserManager<AppUser, string> userManager;

        public ProfileController()
        {
            var context = new AppDbContext();
            var roleStore = new RoleStore<AppRole, string, IdentityUserRole>(context);
            this.roleManager = new RoleManager<AppRole, string>(roleStore);

            var userStore = new UserStore<AppUser, AppRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context);
            this.userManager = new UserManager<AppUser, string>(userStore);
        }

        // Profil Görüntüleme
        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = userManager.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.Name,
                LastName = user.Surname
            };

            return View(model);
        }

        // Profil Güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();
            var user = userManager.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Kullanıcı bilgilerini güncelle
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber ?? string.Empty;
            user.Name = model.FirstName ?? string.Empty;
            user.Surname = model.LastName ?? string.Empty;

            var result = userManager.Update(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();
            var result = userManager.ChangePassword(userId, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userManager != null)
                {
                    userManager.Dispose();
                }
                if (roleManager != null)
                {
                    roleManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}