using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System;
using System.Web.Mvc;

public class LogActionAttribute : ActionFilterAttribute
{
    private readonly string _action;

    public LogActionAttribute(string action)
    {
        _action = action;
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        var user = filterContext.HttpContext.User;

        if (user != null && user.Identity.IsAuthenticated)
        {
            string userName = user.Identity.Name;

            string fullName =
                filterContext.HttpContext.Session["FullName"]?.ToString()
                ?? userName;

            string message = _action == "Login"
                ? $"{fullName} ({userName}) başarıyla giriş yaptı"
                : $"{fullName} ({userName}) sistemden çıkış yaptı";

            using (var context = new AppDbContext())
            {
                context.LoginLogs.Add(new LoginLog
                {
                    LogDate = DateTime.Now,
                    Message = message
                });
                context.SaveChanges();
            }
        }

        base.OnActionExecuted(filterContext);
    }
}
