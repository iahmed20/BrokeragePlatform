using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrokeragePlatform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrokerageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyOrderId",
                table: "Executions");

            migrationBuilder.RenameColumn(
                name: "SellOrderId",
                table: "Executions",
                newName: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Executions",
                newName: "SellOrderId");

            migrationBuilder.AddColumn<long>(
                name: "BuyOrderId",
                table: "Executions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
