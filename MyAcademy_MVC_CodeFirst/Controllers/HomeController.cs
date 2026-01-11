using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using MyAcademy_MVC_CodeFirst.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext context = new AppDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Packages()
        {
            var package = context.Packages.ToList();
            return PartialView(package);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Contact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.SendDate = DateTime.Now;
                contact.IsRead = false;
                context.Contacts.Add(contact);
                context.SaveChanges();

                string autoReplyBody = "";
                try
                {
                    var geminiService = new GeminiService();
                    autoReplyBody = await geminiService.GetSmartReplyAsync(contact.Name, contact.Subject, contact.Message);
                }
                catch (Exception)
                {
                    autoReplyBody = GetFallbackMessage(contact.Name, contact.Subject);
                }

                try
                {
                    var emailService = new EmailService();
                    emailService.SendAutoReply(contact.Email, contact.Name, contact.Subject, autoReplyBody);
                    TempData["SuccessMessage"] = "Mesajınız alındı ve yapay zeka asistanımız tarafından hazırlanan bilgilendirme maili tarafınıza gönderildi!";
                }
                catch (Exception)
                {
                    TempData["SuccessMessage"] = "Mesajınız kaydedildi ancak e-posta gönderilirken bir teknik sorun oluştu.";
                }

                return RedirectToAction("Contact", "Home");
            }
            return View(contact);
        }

        private string GetFallbackMessage(string name, string subject)
        {
            switch (subject)
            {
                case "Teşekkür": return $"Sayın {name}, güzel geri bildiriminiz için teşekkür ederiz.";
                case "Şikayet": return $"Sayın {name}, şikayetiniz birimlerimize iletilmiştir.";
                case "Rica": return $"Sayın {name}, talebiniz işleme alınmıştır.";
                default: return $"Merhaba {name}, mesajınız bize ulaştı. En kısa sürede döneceğiz.";
            }
        }

    }
}