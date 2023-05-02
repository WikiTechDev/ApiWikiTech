using ApiWikiTech.Models;
using ApiWikiTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using ApiWikiTech.Util;


namespace ApiWikiTech.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly IConfiguration _config;
        private readonly JwtConfig _jwtConfig;


        public AuthController(IConfiguration config, MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
            _jwtConfig = new JwtConfig { Key = config["JWT:Key"], Issuer = config["JWT:Issuer"], Audience = config["JWT:Audience"] };
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mongoDBService.GetAsyncUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registro([FromBody] RegisterRequest user)
        {
            try
            {

                if (IToken.ValidateFields(new List<string> { user.Id, user.Username, user.Email, user.Phone, user.Password }))
                    return BadRequest("Es necesario que ingrese todos los campos que sean obligatorios");

                User userDB = new User() { Identification = user.Id, Username = user.Username, Email = user.Email, Phone = user.Phone, Password = user.Password, Role = "user" };
                // Generación del hash y la sal
                using (var hashAlgorithm = new Rfc2898DeriveBytes(user.Password, 32, 10000))
                {
                    byte[] salt = hashAlgorithm.Salt;
                    byte[] hash = hashAlgorithm.GetBytes(256 / 8);

                    // Almacenamiento del hash y la sal en la base de datos
                    userDB.Password = Convert.ToBase64String(hash);
                    userDB.KeyWordHash = Convert.ToBase64String(salt);

                }

                await _mongoDBService.AddUserAsync(userDB, userDB.Role);
                return CreatedAtAction(nameof(GetAllUsers), new { id = userDB.Id }, userDB);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                User UserDB = await _mongoDBService.GetAsyncOneUser(loginRequest.Email);

                if (UserDB == null)
                {
                    return Unauthorized();
                }

                // Verificación de la contraseña
                using (var hashAlgorithm = new Rfc2898DeriveBytes(loginRequest.Password, Convert.FromBase64String(UserDB.KeyWordHash), 10000))
                {
                    byte[] providedHash = hashAlgorithm.GetBytes(256 / 8);
                    bool isPasswordCorrect = IToken.ByteArraysEqual(Convert.FromBase64String(UserDB.Password), providedHash);

                    if (!isPasswordCorrect)
                    {
                        return Unauthorized("Credenciales incorrectas");
                    }
                }
                IToken tokenGenerator = new IToken();
                var response = new
                {
                    token = tokenGenerator.GenerateToken(UserDB, _jwtConfig)
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Authorize]
        [HttpPost("reload_token")]
        public IActionResult RenewToken([FromBody] SessionRequest sessionRequest)
        {
            try
            {
                if (IToken.ValidateFields(new List<string> { sessionRequest.Email })) return BadRequest("Es necesario agregar una direccion de correo valida");
                User user = new User() { Email = sessionRequest.Email, Role = "admin" };
                IToken tokenGenerator = new IToken();
                string newToken = tokenGenerator.GenerateToken(user, _jwtConfig);

                if (string.IsNullOrEmpty(newToken)) throw new Exception("Ocurrió un error al momento de generar el token");
                var newTokenExpirationTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();

                return Ok(new
                {
                    Token = newToken,
                    Expiration = newTokenExpirationTime
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
        /*
        [Authorize]
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var User = _user.Find(u => u.Id == id).FirstOrDefault();
            if (User == null)
            {
                return NotFound();
            }

            return User;
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User UserIn)
        {
            var User = _user.Find(u => u.Id == id).FirstOrDefault();

            if (User == null)
            {
                return NotFound();
            }

            _user.ReplaceOne(u => u.Id == User.Id, UserIn);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var User = _user.Find(u => u.Id == id).FirstOrDefault();

            if (User == null)
            {
                return NotFound();
            }

            _user.DeleteOne(u => u.Id == User.Id);

            return NoContent();
        }
        */
    }
}
