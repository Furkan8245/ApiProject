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
            return View(createMessageDto);
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
            if (string.IsNullOrWhiteSpace(createMessageDto.MessageDetails))
            {
                ViewBag.error = "Mesaj boş olamaz.";
                return View(createMessageDto);
            }

            // 1️⃣ Perspective API ile toksisite analizi
            double toxicityScore = 0.0;
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", perspectiveApiKey);

                var requestObj = new
                {
                    comment = new { text = createMessageDto.MessageDetails },
                    requestedAttributes = new { TOXICITY = new { } }
                };

                string reqJson = JsonSerializer.Serialize(requestObj);
                var content = new StringContent(reqJson, Encoding.UTF8, "application/json");

                var resp = await client.PostAsync("https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key=" + perspectiveApiKey, content);
                if (resp.IsSuccessStatusCode)
                {
                    string respJson = await resp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(respJson);
                    var score = doc.RootElement
                        .GetProperty("attributeScores")
                        .GetProperty("TOXICITY")
                        .GetProperty("summaryScore")
                        .GetProperty("value")
                        .GetDouble();

                    toxicityScore = score;
                }
            }
            catch (Exception ex)
            {
                // Hata olursa message “Onay Bekliyor” olsun
                createMessageDto.Status = "Onay Bekliyor";
                goto SendToBackend;
            }

            // 2️⃣ Toksikse kaydetme
            if (toxicityScore >= 0.5)
            {
                createMessageDto.Status = "Toksik Mesaj";
                goto SendToBackend;
            }

            // 3️⃣ Toksik değilse "Aktif" değil, senin istediğin mantığa göre kaydet
            // Mesaj henüz okunmamış olacak, IsRead = false
            createMessageDto.IsRead = false;
            createMessageDto.SendDate = DateTime.Now;
        // Eğer okunmamışsa Status = NULL (yani burada hiç atama yapma)
        // Yani sadece okununca veya işlem sonrası status atanacak

        SendToBackend:
            {
                // 4️⃣ Mesajı backend’e gönder
                var client2 = _httpClientFactory.CreateClient();
                // Eğer Status boşsa JSON’da null olarak gidecek
                var jsonData = JsonConvert.SerializeObject(createMessageDto,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var resp2 = await client2.PostAsync("https://localhost:7162/api/Messages", stringContent);

                if (resp2.IsSuccessStatusCode)
                {
                    return RedirectToAction("MessageList");
                }
                else
                {
                    string err = await resp2.Content.ReadAsStringAsync();
                    ViewBag.ApiError = err;
                }
            }

            return View(createMessageDto);
        }
    }
}
