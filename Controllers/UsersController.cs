using ApiWikiTech.Models;
using ApiWikiTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using ApiWikiTech.Util;


namespace ApiWikiTech.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public UsersController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mongoDBService.GetAsyncUsers();
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "admin,technician")]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                var currentUser = HttpContext.User;

                switch (currentUser.GetRole())
                {
                    case "admin":
                        await _mongoDBService.AddUserAsync(user, user.Role);
                        return Ok();

                    case "technician":
                        if (user.Role == "user")
                        {
                            await _mongoDBService.AddUserAsync(user, user.Role);
                            return Ok();
                        }
                        else
                        {
                            return Forbid();
                        }

                    default:
                        return Forbid();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, technician, user")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var currentUser = HttpContext.User;
            var userRole = currentUser.GetRole();

            // Si el usuario es admin o técnico, puede ver los datos de cualquier usuario
            // Si el usuario es común, solo puede ver sus propios datos
            if (userRole == "admin" || userRole == "technician")
            {
                var user = await _mongoDBService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            else if (userRole == "user")
            {
                // Obtiene el id del usuario autenticado
                var userId = currentUser.GetUserId();

                // Si el id del usuario autenticado coincide con el id pasado por parámetro, devuelve sus datos
                if (userId == id)
                {
                    var user = await _mongoDBService.GetUserByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return Ok(user);
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }
    }
}
