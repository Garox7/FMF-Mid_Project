using Microsoft.AspNetCore.Mvc;
using SchoolLab.Models;
using SchoolLab.Service;

namespace SchoolLab.Controllers;

[ApiController]
[Route("api/user")]
public class LabUserController : Controller
{
    private readonly HttpClient _httpclient;
    private readonly LabUserService _labUserService;
    
    public LabUserController(LabUserService labUserService)
    {
        _httpclient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5227")
        };

        _labUserService = labUserService;
    }

    [HttpGet]
    [Route("labs")]
    public async Task<ActionResult> GetLabs([FromQuery] string token)
    {
        try{
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                List<Lab> labs = _labUserService.GetAllLabs();

                if (labs is not null)
                {
                    return Ok(labs);
                }
                return NotFound("Labs not found");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet]
    [Route("labs/reservations/{userId}")]
    public async Task<ActionResult> GetReservation(string userId, [FromQuery] string token)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                List<Reservation> userReservation = _labUserService.GetReservationsByUserId(userId);

                if (userReservation is not null)
                {
                    return Ok(userReservation);
                }
                return NotFound("Reservations not found");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("labs/reservations/create")]
    public async Task<ActionResult<Reservation>> CreateReservation([FromBody] ReservationDto reservationDto, [FromQuery] string token)
    {
        try 
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                Reservation newReservation = await _labUserService.BookComputer(reservationDto.LabId, reservationDto.Date, reservationDto.TimeSlot, reservationDto.UserId);

                if (newReservation is not null)
                {
                    return Ok(newReservation);
                }
                return BadRequest();
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("labs/reservations/delete/{reservationId}")]
    public async Task<ActionResult> DeleteReservation(string reservationId, [FromQuery] string userId, [FromQuery] string token)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                if (await _labUserService.CancelBooking(reservationId, userId))
                {
                    return Ok("Booking deleted");
                }
                return BadRequest("Cancellation failed");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}