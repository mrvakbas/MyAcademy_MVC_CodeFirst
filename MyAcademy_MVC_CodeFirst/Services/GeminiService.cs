using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyAcademy_MVC_CodeFirst.Services
{
    public class GeminiService
    {
        private readonly string _apiKey = "AIzaSyA3OiIPVfmdYG_yO_we9kSwLYW8Kx2rFkA";
        private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=";

        public async Task<string> GetSmartReplyAsync(string name, string subject, string message)
        {
            using (var client = new HttpClient())
            {
                var prompt = $"Sen 'LifeSure Sigorta' şirketinin kurumsal yapay zeka asistanısın. " +
              $"Müşteri adı: {name}. Seçtiği konu: {subject}. Mesajı: '{message}'. " +
              $"Bu müşteriye, LifeSure Sigorta adına, seçtiği konuya ve mesaj içeriğine uygun, nazik ve çözüm odaklı bir e-posta cevabı yaz. " +
              $"Cevapta '[Acente Adı]', '[Departman]' gibi yer tutucular ASLA kullanma. " +
              $"Mesajı mutlaka 'Saygılarımızla, LifeSure Sigorta Ekibi' imzasıyla bitir. Maksimum 3-4 cümle olsun.";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_apiUrl + _apiKey, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseJson);
                return result.candidates[0].content.parts[0].text;
            }
        }
    }
}