using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiProjeKampi.WebUI.Controllers
{
    public class AIController : Controller
    {
        public IActionResult CreateRecipeWithGemini()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipeWithGemini(string prompt)
        {
            // 🔑 Kendi Gemini API anahtarın
            var apiKey = "AIzaSyCAQt1Zi9zPJzm9U3eudXgiV5tCJyKiw7Q";


            // 🌍 Gemini'nin metin üretim endpoint'i
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            using var client = new HttpClient();

            // 💬 Kullanıcı prompt'unu içeren istek gövdesi
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text =
                                "Sen bir restoran için yemek önerileri yapan bir yapay zekasın. " +
                                "Kullanıcı tarafından girilen malzemelere göre yaratıcı ve açıklamalı yemek tarifleri öner. " +
                                "Şimdi kullanıcıdan gelen prompt şu: " + prompt
                            }
                        }
                    }
                }
            };

            // JSON olarak serialize et
            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 📡 API isteği gönder
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // 🔍 JSON cevabını çöz
                var jsonObj = JObject.Parse(responseString);
                var generatedText = jsonObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                ViewBag.Recipe = generatedText ?? "Gemini’den bir yanıt alınamadı.";
            }
            else
            {
                ViewBag.Recipe = $"Hata oluştu: {response.StatusCode} - {responseString}";
            }

            return View();
        }
    }
}
