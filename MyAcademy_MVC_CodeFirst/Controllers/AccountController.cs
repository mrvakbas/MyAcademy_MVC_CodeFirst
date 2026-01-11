using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser, string> userManager;
        private RoleManager<AppRole, string> roleManager;

        public AccountController()
        {
            var context = new AppDbContext();

            var userStore = new UserStore<AppUser, AppRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context);
            userManager = new UserManager<AppUser, string>(userStore);

            var roleStore = new RoleStore<AppRole, string, IdentityUserRole>(context);
            roleManager = new RoleManager<AppRole, string>(roleStore);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Kayıt başarılı!";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogAction("Login")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid) return View(model);

            var user = await userManager.FindByNameAsync(model.UserNameOrEmail)
                       ?? await userManager.FindByEmailAsync(model.UserNameOrEmail);

            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, model.Password);
                if (result)
                {
                    using (var context = new AppDbContext())
                    {
                        context.LoginLogs.Add(new LoginLog
                        {
                            LogDate = DateTime.Now,
                            Message = $"{user.Name} {user.Surname} ({user.UserName}) başarıyla giriş yaptı"
                        });
                        context.SaveChanges();
                    }

                    var identity = await userManager.CreateIdentityAsync(
                        user, DefaultAuthenticationTypes.ApplicationCookie);

                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignOut();
                    authManager.SignIn(
                        new AuthenticationProperties { IsPersistent = model.RememberMe },
                        identity
                    );

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    var roles = await userManager.GetRolesAsync(user.Id);

                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    else if (roles.Contains("User"))
                        return RedirectToAction("Index", "DashboardUser", new { area = "User" });

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Kullanıcı adı/Email veya şifre hatalı.");
            return View(model);
        }

        [HttpGet]

        public ActionResult Logout()
        {
            var userName = User.Identity.Name;

            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(x => x.UserName == userName);

                if (user != null)
                {
                    context.LoginLogs.Add(new LoginLog
                    {
                        LogDate = DateTime.Now,
                        Message = $"{user.Name} {user.Surname} ({user.UserName}) sistemden çıkış yaptı"
                    });
                    context.SaveChanges();
                }
            }
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Login", "Account");
        }
    }

    public class RegisterViewModel
    {
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        [Required] public string UserName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
    }

    public class LoginViewModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
