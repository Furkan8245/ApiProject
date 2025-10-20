using ApiProject.WebUI.Dtos.ChefDtos;
using ApiProject.WebUI.Dtos.MessageDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization; // Kültür bilgisi için ekledik

namespace ApiProject.WebUI.ViewComponents.AdminLayoutNavbarViewComponents
{
    public class _NavbarMessageListAdminLayoutComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _NavbarMessageListAdminLayoutComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7162/api/Messages/MessageListByIdReadFalse");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                // Hata Çözümü: Newtonsoft.Json için Deserileştirme Ayarları
                // Gelen tarih formatı (19.10.2025 03:10:12) için format string'ini tanımlıyoruz.
                var settings = new JsonSerializerSettings
                {
                    // Hatanın oluştuğu tarih formatını (gün.ay.yıl saat:dakika:saniye) belirtiyoruz.
                    DateFormatString = "dd.MM.yyyy HH:mm:ss",
                    // Ayrıca, Deserileştirme işleminin Türk kültürünü (tr-TR) kullanmasını sağlayabiliriz.
                    Culture = new CultureInfo("tr-TR")
                };

                // Deserileştirme işlemini güncellenmiş ayarlar ile yapıyoruz.
                var values = JsonConvert.DeserializeObject<List<ResultMessageByIsReadFalse>>(jsonData, settings);

                return View(values);
            }

            return View();
        }
    }
}