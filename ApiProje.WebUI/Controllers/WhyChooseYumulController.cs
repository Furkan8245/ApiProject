using ApiProject.WebUI.Dtos.ServicesDtos;
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
                var value = JsonConvert.DeserializeObject<List<ResultWhyChooseYumuldto>>(jsonData);
                return View(value);
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateServices()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateServices(CreateWhyChooseYumulDto createWhyChooseYumulDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createWhyChooseYumulDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7162/api/Categories", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("ServicesList");
            }
            return View();
        }

        public async Task<IActionResult> DeleteServices(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync("https://localhost:7162/api/Categories?id=" + id);
            return RedirectToAction("ServicesList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateServices(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Categories/GetServices?id=" + id);
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetServicesByIdDto>(jsonData);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateServices(UpdateServicesDto updateServicesDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateServicesDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            await client.PutAsync("https://localhost:7162/api/Categories/", stringContent);
            return RedirectToAction("ServicesList");
        }

    }
}
