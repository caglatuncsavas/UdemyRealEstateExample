using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Context;

namespace RealEstateApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    ApiDbContext _context = new ApiDbContext();

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(_context.Categories);
    }
}
