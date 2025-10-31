using ApiProject.WebUI.Dtos.MessageDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace ApiProject.WebUI.Controllers
{
    public class MessageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MessageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Mesaj Listesi
        public async Task<IActionResult> MessageList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Messages");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultMessageDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        // Mesaj Oluşturma GET
        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        // Mesaj Oluşturma POST
        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
           var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7162/api/Messages", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.ApiError = error;
                return View(createMessageDto);
            }
            return View();
        }

        // Mesaj Silme
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"https://localhost:7162/api/Messages?id={id}");
            return RedirectToAction("MessageList");
        }

        // Mesaj Güncelleme GET
        [HttpGet]
        public async Task<IActionResult> UpdateMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Messages/GetMessage?id=" + id);
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetByIdMessageDto>(jsonData); 
            return View(value);
        }

        // Mesaj Güncelleme POST
        [HttpPost]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            await client.PutAsync("https://localhost:7162/api/Messages/", stringContent);
            return RedirectToAction("MessageList");

        }

        // Mesaja Cevap Görüntüleme
        [HttpGet]
        public async Task<IActionResult> AnswerMessageWithGemini(int id, string prompt)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7162/api/Messages/GetMessage?id={id}");
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetByIdMessageDto>(jsonData);
            prompt = value.MessageDetails;

            var apiKey = "AIzaSyCAQt1Zi9zPJzm9U3eudXgiV5tCJyKiw7Q";


            // 🌍 Gemini'nin metin üretim endpoint'i
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            using var client2 = new HttpClient();

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
                                "Sen bir restoran için kullanıcıların göndermiş oldukları mesajları detaylı ve müşterilere karşı olumlu ve memnun edici cevaplar veren bir yapay zekasın. " + 
                                "Amacımız kullanıcı tarafından gönderilen mesajlara  en olumlu ve en mantıklı cevapları sunabilmektir.  " +
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

                ViewBag.answerAI = generatedText ?? "Gemini’den bir yanıt alınamadı.";
            }
            else
            {
                ViewBag.answerAI = $"Hata oluştu: {response.StatusCode} - {responseString}";
            }
            return View(value);
        }
        public PartialViewResult SendMessage()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            try
            {
                // 1️⃣ LibreTranslate API ile Türkçe -> İngilizce çeviri
                using (var client = new HttpClient())
                {
                    var translateBody = new
                    {
                        q = createMessageDto.MessageDetails, // çevrilecek metin
                        source = "tr",                       // kaynak dil
                        target = "en",                       // hedef dil
                        format = "text"                      // düz metin olarak gönder
                    };

                    var translateJson = System.Text.Json.JsonSerializer.Serialize(translateBody);
                    var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

                    // Ücretsiz çalışan servis (alternatif olarak kendi sunucunu kullanabilirsin)
                    var translateResponse = await client.PostAsync("https://libretranslate.com/translate", translateContent);
                    var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                    // JSON cevabını çözümle
                    if (translateResponse.IsSuccessStatusCode)
                    {
                        var translateDoc = JsonDocument.Parse(translateResponseString);
                        var englishText = translateDoc.RootElement.GetProperty("translatedText").GetString();

                        ViewBag.v = englishText; // Çeviri sonucu ekrana bastırmak için
                        createMessageDto.MessageDetails = englishText; // İstersen çeviriyi veritabanına kaydet
                    }
                    else
                    {
                        ViewBag.error = $"Çeviri hatası: {translateResponse.StatusCode}";
                    }
                }

                // 2️⃣ Veritabanına mesajı kaydetme
                createMessageDto.SendDate = DateTime.Now;
                createMessageDto.IsRead = false;
                createMessageDto.Status = "Aktif"; // API tarafında zorunlu alan

                var client2 = _httpClientFactory.CreateClient();
                var jsonData = JsonConvert.SerializeObject(createMessageDto);
                var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var responseMessage = await client2.PostAsync("https://localhost:7162/api/Messages", stringContent);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("MessageList");
                }

                var error = await responseMessage.Content.ReadAsStringAsync();
                ViewBag.ApiError = error;
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return View(createMessageDto);
        }
    }
}
