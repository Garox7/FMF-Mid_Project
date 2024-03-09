using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolLab.Models;
using SchoolLab.Service;

namespace SchoolLab.Controllers;

[ApiController]
[Route("api/admin")]
public class LabAdminController : Controller
{
    private readonly HttpClient _httpclient;
    private readonly LabAdminService _labAdminService;

    public LabAdminController(LabAdminService labAdminService)
    {
        _httpclient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5227")
        };

        _labAdminService = labAdminService;
    }

    [HttpGet]
    [Route("labs")]
    public async Task<ActionResult> GetLabs([FromQuery] string token)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");
            string userRole = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                List<Lab> labs = _labAdminService.GetAllLabs();

                if (labs is not null)
                {
                    return Ok(labs);
                }
                return NotFound("Labs not Found");
            }
            return Unauthorized("The current user does not have the necessary permissions");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("labs/{labId}")]
    public async Task<ActionResult> GetLab(string labId, [FromQuery] string token)
    {
        var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

        if (response.IsSuccessStatusCode)
        {
            Lab? lab = _labAdminService.GetLabById(labId);

            if(lab is not null)
            {
                return Ok(lab);
            }
            return NotFound("Lab not found");
        }
        return Unauthorized();
    }

    [HttpGet]
    [Route("labs/{labId}/computers")]
    public async Task<ActionResult> GetComputers(string labId, [FromQuery] string token)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");
            
            if (response.IsSuccessStatusCode)
            {
                List<Computer> computers = _labAdminService.GetComputers(labId);

                if (computers is not null)
                {
                    return Ok(computers);
                }
                return NotFound("Computers not found for the selected laboratory");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("labs/{labId}/computers/create")]
    public async Task<ActionResult> CreateComputer(string labId, [FromBody] ComputerDto computer, [FromQuery] string token)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

        if (response.IsSuccessStatusCode)
            {   
                Computer newComputer = await _labAdminService.CreateComputer(labId, computer);

                if (newComputer is not null)
                {
                    return Ok(newComputer);
                }
                return BadRequest("Error while creating");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch]
    [Route("labs/{labId}/computers/update/{computerId}")]
    public async Task<ActionResult> UpdateComputer(string labId, string computerId, [FromBody] ComputerDto newComputerDto, [FromQuery] string token)
    {
        try 
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                Computer computerUpdated = await _labAdminService.UpdateComputer(labId, computerId, newComputerDto);

                if (computerUpdated is not null)
                {
                    return Ok(computerUpdated);
                }
                return NotFound("Computer not found");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("labs/{labId}/computers/delete/{computerId}")]
    public async Task<ActionResult> DeleteComputer(string labId, string computerId, [FromQuery] string token)
    {
        try 
        {
            var response = await _httpclient.GetAsync($"api/authentication/check-auth/{token}");

            if (response.IsSuccessStatusCode)
            {
                if (await _labAdminService.DeleteComputer(labId, computerId))
                {
                    return Ok("Computer Deleted");
                }
                return NotFound("Computer Not found");
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}