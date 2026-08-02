// Controllers/SecuritiesController.cs
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

    // GET /api/securities
    [HttpGet]
    public async Task<IActionResult> ListSecurities()
    {
        var securities = await _db.Securities.ToListAsync();
        return Ok(securities);
    }
    
}