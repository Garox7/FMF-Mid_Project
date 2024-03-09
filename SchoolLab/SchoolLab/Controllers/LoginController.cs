using Microsoft.AspNetCore.Mvc;
using SchoolLab.Models;

namespace SchoolLab.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : Controller
{
    private readonly HttpClient _httpclient;

    public LoginController()
    {
        _httpclient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5227")
        };
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetAutentication(string username)
    {
        var response = await _httpclient.GetAsync($"api/authentication/{username}");

        if (response.IsSuccessStatusCode)
        {
            var challenge = await response.Content.ReadAsStringAsync();
            return Ok(challenge);
        }
        else
        {
            return BadRequest("Invalid credentials");
        }
    }

    [HttpGet]
    [Route("{username}/{challenge}")]
    public async Task<IActionResult> Authentication(string username, string challenge)
    {
        var response = await _httpclient.GetAsync($"api/authentication/{username}/{challenge}");

        if (response.IsSuccessStatusCode)
        {
            UserDto? userDto = await response.Content.ReadFromJsonAsync<UserDto>();

            return Ok(userDto);
        }
        else
        {
            return BadRequest("Invalid credentials");
        }
    }
}