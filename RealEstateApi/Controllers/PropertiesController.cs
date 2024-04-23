using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Context;
using RealEstateApi.Models;
using System.Security.Claims;

namespace RealEstateApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PropertiesController : ControllerBase
{
    ApiDbContext _context = new ApiDbContext();

    [HttpGet("PropertyList")]
    [Authorize]
    public IActionResult GetProperties(int categoryId)
    {
       var propertiesResult = _context.Properties.Where(p => p.CategoryId == categoryId);
        if(propertiesResult is null)
        {
            return NotFound();
        }

        return Ok(propertiesResult);
    }

    [HttpGet("PropertyDetail")]
    [Authorize]
    public IActionResult GetPropertyDetail(int id)
    {
        var propertiesResult = _context.Properties.FirstOrDefault(p => p.Id == id);
        if (propertiesResult is null)
        {
            return NotFound();
        }

        return Ok(propertiesResult);
    }

    [HttpGet("TrendingProperties")]
    [Authorize]
    public IActionResult GetTrendingProperties()
    {
        var propertiesResult = _context.Properties.Where(p => p.IsTrending == true);
        if (propertiesResult is null)
        {
            return NotFound();
        }

        return Ok(propertiesResult);
    }


    [HttpGet("SearchProperties")]
    [Authorize]
    public IActionResult GetSearchProperties(string address)
    {
        var propertiesResult = _context.Properties.Where(p => p.Address.Contains(address));
        if (propertiesResult is null)
        {
            return NotFound();
        }

        return Ok(propertiesResult);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Post([FromBody] Property property)
    {
        if(property is null)
        {
            return NoContent();
        }
        else
        {
           var userEmail =  User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
           var user = _context.Users.FirstOrDefault(p => p.Email == userEmail);
        if(user is null) return NotFound();

            property.IsTrending = false;
            property.UserId = user.Id;
            
         _context.Properties.Add(property);
         _context.SaveChanges();
         return StatusCode(StatusCodes.Status201Created);
         
        }
    }
    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Put(int id, [FromBody] Property property)
    {
       var propertyResult =_context.Properties.FirstOrDefault(p => p.Id == id);
        if (propertyResult is null)
        {
            return NotFound();
        }
        else
        {
            var userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(p => p.Email == userEmail);
            if (user is null) return NotFound();
            //Only the user can update his own record.
            if(propertyResult.UserId == user.Id)
            {
                propertyResult.Name = property.Name;
                propertyResult.Detail = property.Detail;
                propertyResult.Address = property.Address;
                propertyResult.Price = property.Price;

                property.IsTrending = false;
                property.UserId = user.Id;
                _context.SaveChanges();
                return Ok("Record updated successfully");
            }
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var propertyResult = _context.Properties.FirstOrDefault(p => p.Id == id);
        if (propertyResult is null)
        {
            return NotFound();
        }
        else
        {
            var userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(p => p.Email == userEmail);
            if (user is null) return NotFound();
            //Only the user can update his own record.
            if (propertyResult.UserId == user.Id)
            {
                _context.Properties.Remove(propertyResult);
                _context.SaveChanges();
                return Ok("Record deleted successfully");
            }
            return BadRequest();
        }
    }
}

