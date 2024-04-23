using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApi.Context;
using RealEstateApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    ApiDbContext _context = new ApiDbContext();
    private IConfiguration _config;

    public UsersController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("[action]")]
    public IActionResult Register([FromBody] User user)
    {
        var userExists = _context.Users.FirstOrDefault(p => p.Email == user.Email);
        if(userExists is not null)
        {
            return BadRequest("User with same email already exists");
        }
        _context.Users.Add(user);
        _context.SaveChanges();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("[action]")]
    public IActionResult Login([FromBody] User user)
    {
       var currentUser = _context.Users.FirstOrDefault(p => p.Email == user.Email && p.Password == user.Password);
        if (currentUser is null)
        {
            return NotFound();
        }

        var securityKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

       var jwt =  new JwtSecurityTokenHandler().WriteToken(token);

       return Ok(jwt);
    }
}
