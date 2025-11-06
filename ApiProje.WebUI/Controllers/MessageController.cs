using ApiProject.WebUI.Dtos.MessageDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ApiProject.WebUI.Controllers
{
    public class MessageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string perspectiveApiKey = "YOUR_PERSPECTIVE_API_KEY";

        public MessageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> MessageList()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7162/api/Messages");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultMessageDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            // Basit gönderme — bu normal düz CreateMessage kullanılırken
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://localhost:7162/api/Messages", stringContent);
            if (resp.IsSuccessStatusCode)
            {
                return RedirectToAction("MessageList");
            }
            return View();
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync("https://localhost:7162/api/Messages?id=" + id);
            return RedirectToAction("MessageList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7162/api/Messages/GetMessage?id=" + id);
            var json = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetByIdMessageDto>(json);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateMessageDto);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            await client.PutAsync("https://localhost:7162/api/Messages", stringContent);
            return RedirectToAction("MessageList");
        }

        public PartialViewResult SendMessage()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {

            // 1) Çeviri: LibreTranslate kullanımı (örnek)
            string originalText = createMessageDto.MessageDetails;
            string translatedText = originalText;
            try
            {
                using var client = new HttpClient();
                var requestBody = new { q = originalText, source = "tr", target = "en" };
                var bodyJson = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                // LibreTranslate’ın endpoint’i: örnek “https://libretranslate.com/translate”
                var response = await client.PostAsync("https://libretranslate.com/translate", content);
                var respString = await response.Content.ReadAsStringAsync();
                // Burada JSON parse edip translatedText’e alman gerekir
                // … parse kısmı basitleştirildi …
                var doc = JsonDocument.Parse(respString);
                translatedText = doc.RootElement.GetProperty("translatedText").GetString();
            }
            catch
            {
                // Çeviri başarısızsa orijinal metni kullan
                translatedText = originalText;
            }

            // 2) Toksisite kontrolü: örneğin Perspective API veya Detoxify
            bool isToxic = false;
            try
            {
                using var client2 = new HttpClient();
                var toxicRequest = new { comment = new { text = translatedText } };
                var toxicJson = System.Text.Json.JsonSerializer.Serialize(toxicRequest);
                var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");

                // Perspektif API kullanım örneği
                client2.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_KEY");
                var toxicResp = await client2.PostAsync("https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key=YOUR_KEY", toxicContent);
                var toxicStr = await toxicResp.Content.ReadAsStringAsync();
                var doc2 = JsonDocument.Parse(toxicStr);
                var score = doc2.RootElement
                              .GetProperty("attributeScores")
                              .GetProperty("TOXICITY")
                              .GetProperty("summaryScore")
                              .GetProperty("value")
                              .GetDouble();

                if (score > 0.5) isToxic = true;
            }
            catch
            {
                // Hata durumunda toksik olarak işaretleme ya da beklemede bırakabilirsin
                isToxic = false;
            }

            if (isToxic)
                createMessageDto.Status = "Toksik Mesaj";
            else
                createMessageDto.Status = "Mesaj Alındı";

            // 3) Mesajı kaydet
            var client3 = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client3.PostAsync("https://localhost:7162/api/Messages", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("MessageList");
            }
            return View();
        }
    }
}
