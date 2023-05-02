using ApiWikiTech.Models;
using ApiWikiTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ApiWikiTech.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TechniciansController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public TechniciansController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        /*[HttpGet]
        [Authorize(Roles = "admin, technician")]
        public async Task<IActionResult> GetTechnicianData()
        {
            // implementar lógica para obtener los datos del técnico logueado
        }*/

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddTechnician(User technician)
        {
            try
            {
                technician.Role = "technician";
                await _mongoDBService.AddUserAsync(technician, technician.Role);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
