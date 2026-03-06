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

    [HttpGet("with-transactions/{id}")]
    public async Task<ActionResult> GetCategoryWithTransactions(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category == null)
        {
            return NotFound();
        }
        List<Transaction> transactions = await _context.Transactions
            .Where(t => t.TransactionCategory.CategoryId == category.Id && t.Date.Month == DateTime.Now.Month).ToListAsync();
        decimal sum = transactions.Where(t => t.Type == TransactionType.Expense && t.Date.Month == DateTime.Now.Month).Sum(t => t.Amount);

        CategoryWithTransactionsResponse response = new CategoryWithTransactionsResponse
        {
            Id = category.Id,
            Name = category.Name,
            Transactions = transactions,
            MonthlySum = sum
        };
        return Ok(response);
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

    [HttpPost("with-goal")]
    public async Task<ActionResult> PostCategoryWithGoal(CategoryWithGoalRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var category = new Category
        {
            UserId = userId,
            Name = request.Name,
            Color = request.Color,
            Icon = request.Icon,
            CategoryGoals = new List<CategoryGoal> 
            {
                new CategoryGoal 
                {
                    Amount = request.GoalAmount,
                    Period = request.GoalPeriod,
                    StartDate = DateOnly.FromDateTime(DateTime.Now) 
                }
            }
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Mapping to response (the Goal will now have its ID and CategoryId populated)
        var response = new CategoryWithGoalResponse
        {
            Id = category.Id,
            Name = category.Name,
            GoalAmount = category.CategoryGoals.First().Amount,
            GoalPeriod = category.CategoryGoals.First().Period
        };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, response);
    }

    // Put
    [HttpPut("{id}")]
    public async Task<ActionResult> PutCategory(int id, [FromBody] CategoryUpdate updateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Fetch the existing category from the DB
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (existingCategory == null)
        {
            return NotFound("Category not found or you don't have permission.");
        }

        // 2. Only update the fields you want the user to be able to change
        existingCategory.Name = updateDto.Name;
        existingCategory.Color = updateDto.Color;
        existingCategory.Icon = updateDto.Icon;
        existingCategory.Description = updateDto.Description;
        existingCategory.UpdatedAt = DateTime.UtcNow; // Manual update timestamp

        // 3. Save changes (EF only updates what actually changed!)
        await _context.SaveChangesAsync();

        return NoContent();
    }
    // update category and goal at the same time
    // [HttpPut]
    // public async Task<ActionResult> PutCategoryWithTransaction(int id, [FromBody] CategoryUpdate updateDto)

    // Delete
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if(category == null)
        {
            return NotFound();
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }
}