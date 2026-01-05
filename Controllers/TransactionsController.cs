using BankSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace BankSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly BankSystemContext _context;

    public TransactionsController(BankSystemContext context)
    {
        _context = context;
        // await _context.Database.EnsureCreatedAsync();
        _context.Database.EnsureCreated();
    }

    // TODO: Decide which style of implementation is best
    // Do I combine all the functions into the get transaction then filter that way? Or do I have multiple endpoints
    [HttpGet]
    public async Task<ActionResult> GetTransactions([FromQuery] TransactionQueryParameters queryParameters)
    {
        IQueryable<Transaction> transactions = _context.Transactions;

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

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) 
        {
            return NotFound();
        }
        return Ok(transaction);
    }

    // TODO: At some point decide if this will be centered around transactions or around categories, in which case
    // it is probably better to name this something else
    [HttpGet("{store}")]
    public async Task<ActionResult> GetTransactionsByShop(string store)
    {
        var transactions = await _context.Transactions.Where(t => EF.Functions.Contains(t.Store, store)).ToListAsync();
        if (transactions == null)
        {
            return NotFound();
        }
        return Ok(transactions);
    }

    // Post methods
    
    [HttpPost]
    public async Task<ActionResult> PostTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTransaction), new {id = transaction.Id}, transaction);
    }

    // Put Methods

    [HttpPut("{id}")]
    public async Task<ActionResult> PutTransaction(int id, [FromBody] Transaction transaction)
    {
        if(id != transaction.Id)
        {
            return BadRequest();
        }
        
        _context.Entry(transaction).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if(!_context.Transactions.Any(t => t.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent();
    }

    // Delete Methods

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if(transaction == null)
        {
            return NotFound();
        }
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return Ok(transaction);
    }

}