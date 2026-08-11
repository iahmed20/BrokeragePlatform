// Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly BrokerageContext _db;
    private readonly MatchingEngine _matchingEngine;
    public OrdersController(BrokerageContext db, MatchingEngine matchingEngine)
    {
        _db = db;
        _matchingEngine = matchingEngine;
    }

    // GET /api/orders
    [HttpGet]
    public async Task<IActionResult> ListOrders()
    {   
        var orders = await _db.Orders.ToListAsync();
        return Ok(orders);
    }
    public class SubmitOrderRequest
    {
        public int AccountId { get; set; }
        public string Symbol { get; set; } = "";
        public string Side { get; set; } = "";
        public string OrderType { get; set; } = "";
        public decimal? LimitPrice { get; set; }
        public decimal Quantity { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderRequest request)
    {
      if(request.Quantity < 0)
        {
            return BadRequest("Order Quantity must be positive, nyaa.");
        }

        var order = new Order
        {
            AccountId = request.AccountId,
            Symbol = request.Symbol,
            Side = request.Side,
            OrderType = request.OrderType,
            LimitPrice = request.LimitPrice,
            Quantity = request.Quantity
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(); // save first so order.OrderId gets generated

        var result = await _matchingEngine.ExecuteMarketOrder(order);

        return Ok(result);
    }

    // [HttpPost("{id}")]
    // public async Task<IActionResult> MatchEngine(int id)
    // {
    //    var order = await _db.Orders.FindAsync(id);
    //    if (order == null) return NotFound();
    //    var match = await _matchingEngine.ExecuteMarketOrder(order);
    //    return Ok(match);

    // }
   
    
}