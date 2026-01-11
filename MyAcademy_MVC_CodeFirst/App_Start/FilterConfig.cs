using System.Web;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
