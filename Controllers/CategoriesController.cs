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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IQueryable<Category> categories = _context.Categories.Where(c => c.UserId == userId);

        if(!string.IsNullOrEmpty(queryParameters.Name))
        {
            categories = categories.Where(c => c.Name.ToLower().Contains(queryParameters.Name.ToLower()));
        }
        if (!string.IsNullOrEmpty(queryParameters.SortBy))
        {
            if (typeof(Category).GetProperty(queryParameters.SortBy) != null)
            {
                categories = categories.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
            }
        }
        if (!categories.Any())
        {
            return NotFound($"No categories found that met your search criteria. Name: {queryParameters.Name}, SortBy: {queryParameters.SortBy}, OrderBy: {queryParameters.SortOrder}");
        }

        categories.Skip(queryParameters.Size * (queryParameters.Page -1)).Take(queryParameters.Size);

        return Ok(await categories.ToArrayAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategory(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }


    // Post
    [HttpPost]
    public async Task<ActionResult> PostCategory(Category category)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        category.UserId = userId;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategory), new {id = category.Id}, category);
    }

    [HttpPost]
    public async Task<ActionResult> PostCategoryWithGoal(Category category, CategoryGoal categoryGoal)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        category.UserId = userId;
        _context.Categories.Add(category);
        _
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategory), new {id = category.Id}, category);
    }

    // Put

    // Delete
}