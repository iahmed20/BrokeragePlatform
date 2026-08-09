using System.Formats.Tar;
using Microsoft.EntityFrameworkCore;
public class MatchingEngine
{
    private readonly BrokerageContext _db;

    public MatchingEngine(BrokerageContext db) => _db = db;

    public async Task<Order> ExecuteMarketOrder(Order order)
    {
        var price = await GetLatestPrice(order.Symbol);
        var execution = await CreateExecution(order, price, order.Quantity);
        await ApplyLedgerEntries(order, execution);
        UpdateOrderStatus(order, order.Quantity);
        await _db.SaveChangesAsync();
        return order;
    }

    private async Task<decimal> GetLatestPrice(string symbol)
    {
         var priceTick = await _db.PriceTicks
        .Where(p => p.Symbol == symbol)
        .OrderByDescending(p => p.Timestamp)
        .FirstOrDefaultAsync();

        return priceTick?.Price ?? 0;
    }
    private async Task<Execution> CreateExecution(Order order, decimal price, decimal qty)
    {
        var execution = new Execution
        {
            OrderId = order.OrderId,
            Symbol = order.Symbol,
            Price = price,
            Quantity = qty
        };

        _db.Executions.Add(execution);
        await _db.SaveChangesAsync();
        return execution;
        
    }
    private async Task ApplyLedgerEntries(Order order, Execution execution)
    {
        decimal cashAmount = execution.Price * execution.Quantity;
        decimal positionAmount = execution.Quantity;

        if (order.Side == "BUY")
        {
            cashAmount = -cashAmount;
        }
        else 
        {
            positionAmount = -positionAmount;
        }

        var cashEntry = new LedgerEntry
        {
            AccountId = order.AccountId,
            EntryType = "CASH",
            Symbol = null,
            Amount = cashAmount,
            ReferenceType = "TRADE",
            ReferenceId = execution.ExecutionId
        };

        var positionEntry = new LedgerEntry
        {
            AccountId = order.AccountId,
            EntryType = "POSITION",
            Symbol = order.Symbol,
            Amount = positionAmount,
            ReferenceType = "TRADE",
            ReferenceId = execution.ExecutionId
        };

        _db.LedgerEntries.Add(cashEntry);
        _db.LedgerEntries.Add(positionEntry);

        await _db.SaveChangesAsync();
    }
    private void UpdateOrderStatus(Order order, decimal filledQty)
    {
       if (filledQty >= order.Quantity) {
            order.Status = "FILLED";
       } else if (filledQty < order.Quantity)
       {
            order.Status = "PARTIAL";
       }

       order.QuantityFilled = filledQty;

    }

}