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
public class TransactionsController : ControllerBase
{
    private readonly BankSystemContext _context;

    public TransactionsController(BankSystemContext context)
    {
        _context = context;
        // await _context.Database.EnsureCreatedAsync();
        // This is not the proper way to do this apparently 
        // Apparently I make a scope and use the ensure created command there
        _context.Database.EnsureCreated();
    }

    // TODO: Decide which style of implementation is best
    // Do I combine all the functions into the get transaction then filter that way? Or do I have multiple endpoints
    [HttpGet]
    public async Task<ActionResult> GetTransactions([FromQuery] TransactionQueryParameters queryParameters)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IQueryable<Transaction> transactions = _context.Transactions.Where(t => t.UserId == userId);

        if (queryParameters.MinPrice != null)
        {
            transactions = transactions.Where(t => t.Amount >= queryParameters.MinPrice.Value);
        }
        if (queryParameters.MaxPrice != null)
        {
            transactions = transactions.Where(t => t.Amount <= queryParameters.MaxPrice.Value);
        }
        if (!string.IsNullOrEmpty(queryParameters.Store))
        {
            transactions = transactions.Where(t => t.Store.ToLower().Contains(queryParameters.Store.ToLower()));
        }
        if (!string.IsNullOrEmpty(queryParameters.SortBy))
        {
            if (typeof(Transaction).GetProperty(queryParameters.SortBy) != null)
            {
                transactions = transactions.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
            }
        }
        if (!transactions.Any())
        {
            return NotFound($"No transactions found that met your search criteria. MinPrice: {queryParameters.MinPrice}, MaxPrice: {queryParameters.MaxPrice}, Store: {queryParameters.Store}, SortBy: {queryParameters.SortBy}, OrderBy: {queryParameters.SortOrder}");
        }

        transactions.Skip(queryParameters.Size * (queryParameters.Page -1)).Take(queryParameters.Size);

        return Ok(await transactions.ToArrayAsync());
    }
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions([FromQuery] string? shop)
    // {
    //     var query = _context.Transactions.AsQueryable();

    //     if (!string.IsNullOrWhiteSpace(shop))
    //     {
    //         query = query.Where(t => t.Store.Contains(shop));
    //     }

    //     var results = await query.ToListAsync();

    //     if (results == null || !results.Any())
    //     {
    //         return NotFound($"No transactions found for shop: {shop}");
    //     }

    //     return Ok(results);
    // } 

    // Probably add this to the filter
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTransaction(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction == null) 
        {
            return NotFound();
        }
        return Ok(transaction);
    }

    // Post methods
    
    [HttpPost]
    public async Task<ActionResult> PostTransaction(Transaction transaction)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        transaction.UserId = userId;
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTransaction), new {id = transaction.Id}, transaction);
    }

    // Put Methods

    [HttpPut("{id}")]
    public async Task<ActionResult> PutTransaction(int id, [FromBody] TransactionUpdate transactionUpdate)
    {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
       var existingTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

       if (existingTransaction == null)
       {
           return NotFound("Transaction not found or you do not have permission");
       }

       existingTransaction.Date = transactionUpdate.Date;
       existingTransaction.Description = transactionUpdate.Description;
       existingTransaction.Store = transactionUpdate.Store;
       existingTransaction.IsRecurring = transactionUpdate.IsRecurring;
       existingTransaction.Amount = transactionUpdate.Amount;
       existingTransaction.Type = transactionUpdate.Type;
       existingTransaction.IsSplit = transactionUpdate.IsSplit;
       existingTransaction.Notes = transactionUpdate.Notes;

       await _context.SaveChangesAsync();

       return NoContent();
    }

    // Delete Methods

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTransaction(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if(transaction == null)
        {
            return NotFound();
        }
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return Ok(transaction);
    }

}