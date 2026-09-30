using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrokeragePlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityGbmParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Drift",
                table: "Securities",
                type: "REAL",
                nullable: false,
                defaultValue: 0.05);

            migrationBuilder.AddColumn<double>(
                name: "Volatility",
                table: "Securities",
                type: "REAL",
                nullable: false,
                defaultValue: 0.20);

            // Give each seeded symbol its own drift/volatility profile
            migrationBuilder.Sql("UPDATE Securities SET Drift = 0.08, Volatility = 0.20 WHERE Symbol = 'NEKO';");
            migrationBuilder.Sql("UPDATE Securities SET Drift = 0.05, Volatility = 0.15 WHERE Symbol = 'PAWS';");
            migrationBuilder.Sql("UPDATE Securities SET Drift = 0.12, Volatility = 0.35 WHERE Symbol = 'MEOW';");
            migrationBuilder.Sql("UPDATE Securities SET Drift = 0.03, Volatility = 0.10 WHERE Symbol = 'TUNA';");
            migrationBuilder.Sql("UPDATE Securities SET Drift = -0.02, Volatility = 0.50 WHERE Symbol = 'YARN';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Drift",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "Volatility",
                table: "Securities");
        }
    }
}
