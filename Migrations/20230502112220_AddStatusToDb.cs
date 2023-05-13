using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhicoin.Migrations
{
    public partial class AddStatusToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "KYCVerificationTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "KYCVerificationTokens");
        }
    }
}
