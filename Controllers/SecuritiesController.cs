// Controllers/SecuritiesController.cs
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SecuritiesController : ControllerBase
{
    private readonly BrokerageContext _db;

    public SecuritiesController(BrokerageContext db)
    {
        _db = db;
    }


   [HttpGet]
    public async Task<IActionResult> GetPrices()
    {
        var allTicks = await _db.PriceTicks
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();

        var grouped = allTicks
            .GroupBy(p => p.Symbol)
            .Select(g => g.Take(30).OrderBy(p => p.Timestamp).ToList());

        return Ok(grouped);
    }

    [HttpGet("{symbol}/prices")]
    public async Task<IActionResult> GetPricesForSymbol(string symbol)
    {
        var ticks = await _db.PriceTicks
            .Where(p => p.Symbol == symbol)
            .OrderByDescending(p => p.Timestamp)
            .Take(30)
            .ToListAsync();

        ticks.Reverse(); // put back in chronological order (oldest -> newest) for the chart

        return Ok(ticks);
    }
    

}