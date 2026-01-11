using MyAcademy_MVC_CodeFirst.Data.Context;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using Nest;
using System.Net;
using Elasticsearch.Net;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogController : Controller
    {
        private AppDbContext context = new AppDbContext();
        private readonly IElasticClient _client;

        public LogController()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("login-logs");

            _client = new ElasticClient(settings);
        }

        public ActionResult Index()
        {
            var allLogs = context.LoginLogs.OrderByDescending(x => x.LogDate).ToList();

            SyncLogsToElastic(allLogs);

            ViewBag.TotalCount = allLogs.Count;
            ViewBag.LoginCount = allLogs.Count(x => x.Message.ToLower().Contains("giriş"));
            ViewBag.LogoutCount = allLogs.Count(x => x.Message.ToLower().Contains("çıkış"));
            ViewBag.ActiveCount = allLogs.Count(x => x.Message.ToLower().Contains("aktif"));
            ViewBag.PassiveCount = allLogs.Count(x => x.Message.ToLower().Contains("pasif"));

            return View(allLogs);
        }

        private void SyncLogsToElastic(List<LoginLog> logs)
        {
            var bulkRequest = new BulkDescriptor();

            foreach (var item in logs)
            {
                // Elastic için ID’yi int’den string’e çeviriyoruz
                // Böylece duplicate => overwrite olur
                bulkRequest
                    .Index<LoginLog>(op => op
                        .Id(item.Id.ToString())
                        .Document(item)
                        .Index("login-logs")
                    );
            }

            var bulkResponse = _client.Bulk(bulkRequest);

            if (bulkResponse.Errors)
            {
                foreach (var itemWithError in bulkResponse.ItemsWithErrors)
                {
                    System.Diagnostics.Debug.WriteLine($"Error indexing {itemWithError.Id}: {itemWithError.Error}");
                }
            }
        }

        public ActionResult DeleteLog(int id)
        {
            var log = context.LoginLogs.Find(id);

            if (log != null)
            {
                context.LoginLogs.Remove(log);
                context.SaveChanges();

                // Elastic'ten de sil
                _client.Delete<LoginLog>(id.ToString(), d => d.Index("login-logs"));
            }

            return RedirectToAction("Index");
        }
    }
}
