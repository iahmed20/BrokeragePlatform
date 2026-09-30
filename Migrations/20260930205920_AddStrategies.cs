using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrokeragePlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddStrategies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    StrategyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.StrategyId);
                });

            migrationBuilder.CreateTable(
                name: "StrategySubmissions",
                columns: table => new
                {
                    StrategySubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategySubmissions", x => x.StrategySubmissionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Strategies_AccountId",
                table: "Strategies",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StrategySubmissions_AccountId",
                table: "StrategySubmissions",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "StrategySubmissions");
        }
    }
}
