using ApiProject.WebUI.Dtos.MessageDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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
        public async Task<IActionResult> AnswerMessageWithGemini(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7162/api/Messages/GetMessage?id={id}");
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<UpdateMessageDto>(jsonData); // Burayı da UpdateMessageDto yaptık
            return View(value);
        }
    }
}
