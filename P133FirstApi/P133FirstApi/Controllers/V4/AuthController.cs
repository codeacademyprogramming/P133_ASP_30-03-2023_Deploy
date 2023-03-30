using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using P133FirstApi.DTOs.AuthDTOs;
using P133FirstApi.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace P133FirstApi.Controllers.V4
{
    [Route("api/v4/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<AppUser> userManager,IConfiguration configuration)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _config = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            AppUser appUser = _mapper.Map<AppUser>(registerDto);

            await _userManager.CreateAsync(appUser,registerDto.Password);

            await _userManager.AddToRoleAsync(appUser, "Member");

            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(loginDto.Email);

            if (appUser == null) return BadRequest();

            if (!await _userManager.CheckPasswordAsync(appUser,loginDto.Password))
            {
                return BadRequest();
            }

            var ıdentityRoles = await _userManager.GetRolesAsync(appUser);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim("Adi",appUser.Name)
            };

            foreach (var item in ıdentityRoles)
            {
                Claim claim = new Claim(ClaimTypes.Role, item);

                claims.Add(claim);
            }

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("JwtSetting:SecretKey").Value));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(4));

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            return Ok(new TokenDTO { Token = token});
        }

        [HttpGet]
        public async Task<IActionResult> ReadToken()
        {
            string token = HttpContext.Request.Headers.Authorization.ToString().Split(' ')[1];

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            JwtSecurityToken test = (JwtSecurityToken)jwtSecurityTokenHandler.ReadToken(token);

            var email = test.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            return Ok(email);
        }

        //[HttpGet]
        //[Route("createRole")]
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //    return Ok();
        //}
    }
}
