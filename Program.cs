using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BrokerageContext>(options =>
    options.UseSqlite("Data Source=brokerage.db"));

builder.Services.AddControllers();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BrokerageContext>();
    if (!db.Securities.Any())
    {
        db.Securities.Add(new Security { Symbol = "NEKO", Name = "Neko Corp" });
        db.SaveChanges();
    }
}
app.MapControllers();
app.Run();