using ApiProject.WebApi.Context;
using ApiProject.WebApi.Dtos.ReservationDtos;
using ApiProject.WebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public ReservationsController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult ReservationList()
        {
            var values = _context.Reservations.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateReservation(CreateResevationDto createReservationDto)
        {
            //_context.Reservations.Add(Reservation);
            //_context.SaveChanges();
            var value = _mapper.Map<Reservation>(createReservationDto);
            _context.Reservations.Add(value);
            _context.SaveChanges();
            return Ok("Rezervasyon ekleme işlemi başarı ile kaydedildi.");
        }
        [HttpDelete]
        public IActionResult DeleteReservation(int id)
        {
            var value = _context.Reservations.Find(id);
            _context.Reservations.Remove(value);
            _context.SaveChanges();
            return Ok("Rezervasyon silme işlemi başarı ile kaydedildi.");
        }
        [HttpGet("GetReservation")]
        public IActionResult GetReservation(int id)
        {
            var value = _context.Reservations.Find(id);
            return Ok(value);

        }
        [HttpPut]
        public IActionResult UpdateReservation(UpdateResevationDto updateReservationDto)
        {
            var value = _mapper.Map<Reservation>(updateReservationDto);
            _context.Reservations.Update(value);
            _context.SaveChanges();
            return Ok("Rezervasyon güncelleme işlemi başarı ile kaydedildi.");
        }

    }
}
