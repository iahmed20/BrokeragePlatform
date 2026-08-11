// Services/PriceTickerService.cs
using Microsoft.EntityFrameworkCore;

public class PriceTickerService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly Random _random = new();

    public PriceTickerService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BrokerageContext>();
                var symbols = await db.Securities.Select(s => s.Symbol).ToListAsync();

                foreach (var symbol in symbols)
                {
                    var lastTick = await db.PriceTicks
                        .Where(p => p.Symbol == symbol)
                        .OrderByDescending(p => p.Timestamp)
                        .FirstOrDefaultAsync();

                    decimal lastPrice = lastTick?.Price ?? 100m; 
                    decimal change = (decimal)(_random.NextDouble() - 0.5) * 2;
                    decimal newPrice = Math.Max(1, lastPrice + change); 

                    db.PriceTicks.Add(new PriceTick { Symbol = symbol, Price = newPrice });
                }

                await db.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); 
        }
    }
} 