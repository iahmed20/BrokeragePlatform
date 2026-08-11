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

    // // GET /api/securities
    // [HttpGet]
    // public async Task<IActionResult> ListSecurities()
    // {   
        
    //     var securities = await _db.Securities.ToListAsync();
    //     return Ok(securities);
    // }

    [HttpGet]
    public async Task<IActionResult> GetPrices()
    {    
        var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
        var ticks = await _db.PriceTicks
            .Where(p => p.Timestamp >= fiveMinutesAgo)
            .OrderBy(p => p.Timestamp)
            .ToListAsync();

        var grouped = ticks.GroupBy(p => p.Symbol);
            
        return Ok(grouped);
    }

}