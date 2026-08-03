using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BrokerageContext>(options =>
    options.UseSqlite("Data Source=brokerage.db"));
    
builder.Services.AddControllers();
builder.Services.AddHostedService<PriceTickerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BrokerageContext>();

    if (!db.Securities.Any())
    {
        db.Securities.AddRange(
            new Security { Symbol = "NEKO", Name = "Neko Corp" },
            new Security { Symbol = "PAWS", Name = "Paws & Co" },
            new Security { Symbol = "MEOW", Name = "Meow Industries" },
            new Security { Symbol = "TUNA", Name = "Tuna Holdings" },
            new Security { Symbol = "YARN", Name = "Yarn Dynamics" }
        );
        db.SaveChanges();
    }

    if (!db.Accounts.Any())
    {
        var alice = new Account { OwnerName = "Alice Trader" };
        var bob = new Account { OwnerName = "Bob Investor" };

        db.Accounts.AddRange(alice, bob);
        db.SaveChanges(); 

        db.LedgerEntries.AddRange(
            new LedgerEntry {
                AccountId = alice.AccountId, EntryType = "CASH",
                Amount = 10000, ReferenceType = "DEPOSIT", ReferenceId = 0
            },
            new LedgerEntry {
                AccountId = bob.AccountId, EntryType = "CASH",
                Amount = 5000, ReferenceType = "DEPOSIT", ReferenceId = 0
            }
        );
        db.SaveChanges();
    }
}

app.MapControllers();
app.Run();