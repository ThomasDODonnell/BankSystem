using BankSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using System.Security.Claims;             // For User.FindFirstValue

namespace BankSystem.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly BankSystemContext _context;
    public CategoriesController(BankSystemContext context)
    {
        _context = context;
        // await _context.Database.EnsureCreatedAsync();
        // This is not the proper way to do this apparently 
        // Apparently I make a scope and use the ensure created command there
        _context.Database.EnsureCreated();
    }

    // Get
    [HttpGet]
    public async Task<ActionResult> GetCategories([FromQuery] CategoryQueryParameters queryParameters)
    {

    }

    // Post
    [HttpPost]
    public async Task<ActionResult> PostCategory(Category category)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        category.UserId = userId;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategories), new {id = category.id}, category);
    }

    // Put

    // Delete
}