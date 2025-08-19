using ApiProject.WebApi.Context;
using ApiProject.WebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YumulEventsController : ControllerBase
    {
        private readonly ApiContext _context;

        public YumulEventsController(ApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult YumulEventList()
        {
            var values = _context.YumulEvents.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateYumulEvent(YumulEvent YumulEvent)
        {
            _context.YumulEvents.Add(YumulEvent);
            _context.SaveChanges();
            return Ok("Etkinlik ekleme işlemi başarı ile kaydedildi.");
        }
        [HttpDelete]
        public IActionResult DeleteYumulEvent(int id)
        {
            var value = _context.YumulEvents.Find(id);
            _context.YumulEvents.Remove(value);
            _context.SaveChanges();
            return Ok("Etkinlik silme işlemi başarı ile kaydedildi.");
        }
        [HttpGet("GetYumulEvent")]
        public IActionResult GetYumulEvent(int id)
        {
            var value = _context.YumulEvents.Find(id);
            return Ok(value);

        }
        [HttpPut]
        public IActionResult UpdateYumulEvent(YumulEvent YumulEvent)
        {
            _context.YumulEvents.Update(YumulEvent);
            _context.SaveChanges();
            return Ok("Etkinlik güncelleme işlemi başarı ile kaydedildi.");
        }
    }
}
