using BankSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using System.Security.Claims;

namespace BankSystem.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TransactionCategoryController : ControllerBase
{
    private readonly BankSystemContext _context;
    public TransactionCategoryController(BankSystemContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }
    //Get methods
    //Put Methods
    //Post Methods
    [HttpPost]
    public async Task<ActionResult> CategorizeTransaction(TTnsaractionCategoryParametersnsaractionCategoryParameters request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var transactionCategory = new TransactionCategory
        {
            TransactionId = request.TransactionId,
            CategoryId = request.TransactionId
        };
        _context.TransactionCategories.Add(transactionCategory);
        await _context.SaveChangesAsync();
    }
    //Delete Methods
}