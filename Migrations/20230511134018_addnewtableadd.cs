using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhicoin.Migrations
{
    public partial class addnewtableadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellCurrencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    btcAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MvrAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrenyChain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencySellDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyTakeFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellCurrencies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellCurrencies");
        }
    }
}
