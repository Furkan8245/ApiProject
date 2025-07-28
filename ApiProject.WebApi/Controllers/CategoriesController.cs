using ApiProject.WebApi.Context;
using ApiProject.WebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiContext _context;

        public CategoriesController(ApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult CategoryList()
        {
            var values = _context.Categories.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok("Kategori ekleme işlemi başarı ile kaydedildi.");
        }
        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            var value= _context.Categories.Find(id);
            _context.Categories.Remove(value);
            _context.SaveChanges();
            return Ok("Kategori silme işlemi başarı ile kaydedildi.");
        }
        [HttpGet("GetCategory")]
        public IActionResult GetCategory(int id) 
        {
            var value = _context.Categories.Find(id);
            return Ok(value);

        }
        [HttpPut]
        public IActionResult UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return Ok("Kategori güncelleme işlemi başarı ile kaydedildi.");
        }

    }
}
