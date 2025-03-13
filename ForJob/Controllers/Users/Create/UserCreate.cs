using FluentValidation;
using ForJob.DbContext;
using ForJob.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Security.Cryptography;

namespace ForJob.Controllers.Users.Create
{
    [AllowAnonymous]
    [Route("api/user")]
    [ApiController]
    public class UserCreate : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IValidator<UserCreateRequest> _userValidator;

        public UserCreate(DatabaseContext context, IValidator<UserCreateRequest> userValidator)
        {
            _context = context;
            _userValidator = userValidator;
        }

        [HttpPost]
        public async Task<ActionResult> Post(UserCreateRequest req)
        {
            var validationResult = await _userValidator.ValidateAsync(req);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            CreatePassHash(req.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Name = req.Name,
                Role = req.Role,
                Hash = passwordHash,
                Salt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created();
        }

        private void CreatePassHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
