// Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly BrokerageContext _db;

    public OrdersController(BrokerageContext db)
    {
        _db = db;
    }

    // GET /api/orders
    [HttpGet]
    public async Task<IActionResult> ListOrders()
    {   //get orders would be from the ledger
        var securities = await _db.Securities.ToListAsync();
        return Ok(securities);
    }

    // [HttpPost("{id}/order")]
    // public async Task<IActionResult> MatchEngine ()
    // {
        

    // }


    
}