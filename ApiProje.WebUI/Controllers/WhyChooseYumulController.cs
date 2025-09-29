using ApiProject.WebUI.Dtos.WhyChooseYumulDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ApiProject.WebUI.Controllers
{
    public class WhyChooseYumulController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WhyChooseYumulController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> WhyChooseYumulList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Services");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultWhyChooseYumulDto>>(jsonData);
                return View(value);
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateWhyChooseYumul()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateWhyChooseYumul(CreateWhyChooseYumulDto createWhyChooseYumulDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createWhyChooseYumulDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7162/api/Services", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("WhyChooseYumulList");
            }
            return View();
        }

        public async Task<IActionResult> DeleteWhyChooseYumul(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync("https://localhost:7162/api/Services?id=" + id);
            return RedirectToAction("WhyChooseYumulList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateWhyChooseYumul(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Services/GetServices?id=" + id);
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetWhyChooseYumulByIdDto>(jsonData);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateWhyChooseYumul(UpdateWhyChooseYumulDto updateWhyChooseYumulDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateWhyChooseYumulDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            await client.PutAsync("https://localhost:7162/api/Services/", stringContent);
            return RedirectToAction("WhyChooseYumulList");
        }

    }
}
