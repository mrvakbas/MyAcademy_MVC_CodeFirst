using MyAcademy_MVC_CodeFirst.Areas.Admin.Data;
using MyAcademy_MVC_CodeFirst.Data.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MyAcademy_MVC_CodeFirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        AppDbContext context = new AppDbContext();

        public ActionResult Index()
        {
            var userRoleId = context.Roles.FirstOrDefault(r => r.Name == "User")?.Id;

            ViewBag.TotalPolicies = context.Policies.Count();
            ViewBag.TotalUsers = context.Users
                .Where(x => x.Roles.Any(r => r.RoleId == userRoleId))
                .Count();
            ViewBag.TotalCategories = context.Categories.Count();
            ViewBag.TotalRevenue = context.Policies.Sum(x => (decimal?)x.Price) ?? 0;
            ViewBag.ActivePolicies = context.Policies.Count(x => x.IsActive);
            ViewBag.ExpiredPolicies = context.Policies.Count(x => !x.IsActive);

            // Son 6 aylık veri
            DateTime sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var stats = context.Policies
                .Where(p => p.StartDate >= sixMonthsAgo)
                .GroupBy(p => new { p.StartDate.Year, p.StartDate.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToList()
                .Select(x => new {
                    Period = x.Year + "-" + x.Month.ToString("D2"),
                    x.Count
                })
                .OrderBy(x => x.Period)
                .ToList();

            ViewBag.Labels = JsonConvert.SerializeObject(stats.Select(x => x.Period).ToList());
            ViewBag.Data = JsonConvert.SerializeObject(stats.Select(x => x.Count).ToList());

            // Kategori dağılımı
            var categoryStats = context.Policies
                .GroupBy(p => p.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.CategoryLabels = JsonConvert.SerializeObject(categoryStats.Select(x => x.CategoryName).ToList());
            ViewBag.CategoryData = JsonConvert.SerializeObject(categoryStats.Select(x => x.Count).ToList());

            // Son eklenen 5 poliçe
            var lastPolicies = context.Policies
                .Include(x => x.AppUser)
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToList();

            ViewBag.LastPolicies = lastPolicies;
            ViewBag.CityForecasts = GetCityForecastTrend(3);

            // --- Mesaj Kategorileri (Contact) İstatistikleri Başlangıç ---

            var contactStats = context.Contacts
          .GroupBy(x => x.Subject)
          .Select(g => new SubjectStatDto // Buradaki isimler Subject ve Count
          {
              Subject = g.Key ?? "Diğer",
              Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToList();

            ViewBag.ContactStatsList = contactStats;
            // JS için serileştirme
            ViewBag.ContactLabels = JsonConvert.SerializeObject(contactStats.Select(x => x.Subject).ToList());
            ViewBag.ContactData = JsonConvert.SerializeObject(contactStats.Select(x => x.Count).ToList());

            // --- Mesaj Kategorileri İstatistikleri Bitiş ---

            return View(lastPolicies);
        }

        private Dictionary<string, int[]> GetCityForecastTrend(int monthsToForecast)
        {
            var result = new Dictionary<string, int[]>();

            var cities = context.Policies
                .Select(x => x.City)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            foreach (var city in cities)
            {
                var monthlyData = context.Policies
                    .Where(p => p.City == city)
                    .GroupBy(p => new { p.StartDate.Year, p.StartDate.Month })
                    .Select(g => new {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

                if (monthlyData.Count == 0)
                    continue;

                var last6 = monthlyData.Skip(Math.Max(0, monthlyData.Count - 6)).ToList();

                double weightedSum = 0, totalWeight = 0;
                double weight = 1;
                foreach (var d in last6)
                {
                    weightedSum += d.Count * weight;
                    totalWeight += weight;
                    weight++;
                }
                double weightedAvg = weightedSum / totalWeight;

                int[] forecast = new int[monthsToForecast];
                for (int i = 0; i < monthsToForecast; i++)
                {
                    double trendFactor = 1 + (i * 0.05);
                    forecast[i] = (int)Math.Round(weightedAvg * trendFactor);
                }

                result[city] = forecast;
            }
            return result;
        }
    }
}
