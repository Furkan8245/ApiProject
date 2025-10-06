using ApiProject.WebUI.Dtos.YumulEventDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ApiProject.WebUI.Controllers
{
    public class YumulEventController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public async Task<IActionResult> YumulEventList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/YumulEvents");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultYumulEventDto>>(jsonData);
                return View(value);
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateYumulEvent()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateYumulEvent(CreateYumulEventDto createYumulEventDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createYumulEventDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7162/api/YumulEvents", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("YumulEventList");
            }
            return View();
        }

        public async Task<IActionResult> DeleteYumulEvent(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync("https://localhost:7162/api/YumulEvents?id=" + id);
            return RedirectToAction("YumulEventList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateYumulEvent(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/YumulEvents/GetYumulEvent?id=" + id);
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<GetYumulEventByIdDto>(jsonData);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateYumulEvent(UpdateYumulEventDto updateYumulEventDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateYumulEventDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            await client.PutAsync("https://localhost:7162/api/YumulEvents/", stringContent);
            return RedirectToAction("YumulEventList");
        }


    }
}
