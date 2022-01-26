using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Services;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExistsAsync(registerDTO.username)) return BadRequest("Username alreay taken !");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Username = registerDTO.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                username = user.Username,
                token = _tokenService.createToken(user)
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(users => users.Username == loginDTO.username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password !");
            }

            return new UserDTO
            {
                username = user.Username,
                token = _tokenService.createToken(user)
            };
        }

        private async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(users => users.Username == username.ToLower());
        }
    }
}