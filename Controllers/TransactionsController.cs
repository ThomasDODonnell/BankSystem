using BankSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet]
    public async Task<ActionResult> GetTransactions()
    {
        return Ok(await _context.Transactions.ToArrayAsync());
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions([FromQuery] string? shop)
    {
        var query = _context.Transactions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(shop))
        {
            query = query.Where(t => t.Store.Contains(shop));
        }

        var results = await query.ToListAsync();

        if (results == null || !results.Any())
        {
            return NotFound($"No transactions found for shop: {shop}");
        }

        return Ok(results);
    } 

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

}