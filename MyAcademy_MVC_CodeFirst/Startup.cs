using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(MyAcademy_MVC_CodeFirst.Startup))]

namespace MyAcademy_MVC_CodeFirst
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cookie Authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = System.TimeSpan.FromDays(7),
                SlidingExpiration = true
            });
        }
    }
}