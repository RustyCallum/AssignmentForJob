using ForJob.DbContext;
using ForJob.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ForJob.Controllers.Users.Login
{
    [AllowAnonymous]
    [Route("api/user/login")]
    [ApiController]
    public class UserLogin : ControllerBase
    {
        private const string TokenSecretKey = "Aleksander51HaHaXDbekazwasxDxDnienawidzewas";
        private readonly DatabaseContext _context;

        public UserLogin(DbContext.DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login(UserLoginRequest req)
        {
            Guid g = Guid.NewGuid();

            if (req == null)
            {
                return BadRequest("No request body");
            }

            var userToCheck = _context.Users.Where(x => x.Name == req.Name).FirstOrDefault();

            if (userToCheck == null)
            {
                return BadRequest("User doesn't exist");
            }

            if (!VerifyPassword(req.Password, userToCheck.Hash, userToCheck.Salt))
            {
                return BadRequest("Wrong password");
            }

            string token = GenerateToken(userToCheck, g);

            return Ok(token);
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateToken(User request, Guid g)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecretKey);

            var claims = new List<Claim>
            {
                new("Name", request.Name),
                new("Role", request.Role),
                new("UserId", request.Id.ToString()),
                new("X-CSRF-TOKEN", g.ToString()),
                new(ClaimTypes.Role, request.Role),
            };

            var TokenSettings = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = "http://localhost:7094/",
                Issuer = "http://localhost:7094/",
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(TokenSettings);
            var jwt = tokenHandler.WriteToken(token);

            Response.Headers.Append("X-CSRF-TOKEN", g.ToString());

            Response.Cookies.Append("token", jwt,
                new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None
                });

            return jwt;
        }
    }
}
