using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAcademy_MVC_CodeFirst.Areas.Admin.Models;
using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleManager<AppRole, string> roleManager;
        private UserManager<AppUser, string> userManager;

        public RoleController()
        {
            var context = new AppDbContext();

            // RoleStore yapılandırması
            var roleStore = new RoleStore<AppRole, string, IdentityUserRole>(context);
            roleManager = new RoleManager<AppRole, string>(roleStore);

            // UserStore yapılandırması (6 parametreli tam yapı)
            var userStore = new UserStore<AppUser, AppRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context);
            userManager = new UserManager<AppUser, string>(userStore);
        }

        public ActionResult Index()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Rol adı boş olamaz!");
                return View();
            }

            if (roleManager.RoleExists(roleName))
            {
                ModelState.AddModelError("", "Bu rol zaten mevcut!");
                return View();
            }

            try
            {
                // AppRole nesnesini ID ve Name ile birlikte oluşturuyoruz
                var newRole = new AppRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName
                };

                // Oluşturduğumuz nesneyi parametre olarak gönderiyoruz
                var result = roleManager.Create(newRole);

                if (result.Succeeded)
                {
                    TempData["Success"] = $"'{roleName}' rolü başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }

                // Eğer Identity tarafından bir hata dönerse (şifre politikası vb. gibi, gerçi rolde pek olmaz ama)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", $"Alan: {validationError.PropertyName} - Hata: {validationError.ErrorMessage}");

                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Update(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            var role = roleManager.FindById(id);
            if (role == null) return HttpNotFound();

            return View(role);
        }

        [HttpPost]
        public ActionResult Update(AppRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = roleManager.FindById(role.Id);
                if (existingRole != null)
                {
                    existingRole.Name = role.Name;
                    var result = roleManager.Update(existingRole);

                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Rol başarıyla güncellendi!";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(role);
        }

        public ActionResult Delete(string id)
        {
            var role = roleManager.FindById(id);
            if (role != null)
            {
                var result = roleManager.Delete(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Rol başarıyla silindi!";
                }
                else
                {
                    TempData["Error"] = "Rol silinemedi: " + string.Join(", ", result.Errors);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ManageUserRoles()
        {
            var users = userManager.Users.ToList();
            var model = users.Select(user => new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                SelectedRoles = userManager.GetRoles(user.Id).ToList()
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name 
                };

                var result = userManager.Create(newUser, model.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(newUser.Id, "User");
                    TempData["Success"] = "Yeni kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction("ManageUserRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            var user = userManager.FindById(id);
            if (user != null)
            {
                var result = userManager.Delete(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Kullanıcı başarıyla silindi.";
                }
                else
                {
                    TempData["Error"] = "Kullanıcı silinirken hata oluştu: " + string.Join(", ", result.Errors);
                }
            }
            return RedirectToAction("ManageUserRoles");
        }

        [HttpGet]
        public ActionResult UpdateUser(string id)
        {
            var user = userManager.FindById(id);
            if (user == null) return HttpNotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateUser(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindById(model.Id);
                if (user == null) return HttpNotFound();

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Name = model.Name;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);
                }

                var result = userManager.Update(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Kullanıcı bilgileri (ve şifresi) güncellendi.";
                    return RedirectToAction("ManageUserRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult EditUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return HttpNotFound();

            var user = userManager.FindById(userId);
            if (user == null) return HttpNotFound();

            // Kullanıcının mevcut rolleri
            var userRoles = userManager.GetRoles(userId);

            // SİSTEMDEKİ TÜM ROLLER (Burayı kontrol et!)
            var allRoles = roleManager.Roles.ToList();

            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                SelectedRoles = userRoles.ToList(), // Kullanıcının sahip olduğu roller
                AllRoles = allRoles.ToList(), // Görünmeyen kısım burası!
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUserRoles(UserRolesViewModel model)
        {
            // 1. Kullanıcıyı bulurken doğru UserManager (AppUser, string) kullanıldığından emin oluyoruz
            var user = userManager.FindById(model.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            try
            {
                // 2. Mevcut rolleri al
                var currentRoles = userManager.GetRoles(model.UserId).ToList();

                // 3. Mevcut rolleri sil (Eğer varsa)
                if (currentRoles.Any())
                {
                    var removeResult = userManager.RemoveFromRoles(model.UserId, currentRoles.ToArray());
                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Eski roller silinirken hata oluştu.");
                        return View(model);
                    }
                }

                // 4. Yeni seçilen rolleri ekle
                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    var addResult = userManager.AddToRoles(model.UserId, model.SelectedRoles.ToArray());
                    if (!addResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Yeni roller eklenirken hata oluştu.");
                        return View(model);
                    }
                }

                TempData["Success"] = $"{user.UserName} kullanıcısının rolleri başarıyla güncellendi!";
                return RedirectToAction("ManageUserRoles");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Eğer veritabanı kısıtlamasına takılırsa hatayı yakala
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                    {
                        ModelState.AddModelError("", $"Hata: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu: " + ex.Message);
            }

            var originalUser = userManager.FindById(model.UserId);
            model.UserName = originalUser.UserName;
            model.UserEmail = originalUser.Email;
            model.AllRoles = roleManager.Roles.ToList();
            return View(model);
        }
    }
}