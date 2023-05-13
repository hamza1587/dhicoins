using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhicoin.Migrations
{
    public partial class AddformurlToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "form_url",
                table: "KYCVerificationTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "form_url",
                table: "KYCVerificationTokens");
        }
    }
}
