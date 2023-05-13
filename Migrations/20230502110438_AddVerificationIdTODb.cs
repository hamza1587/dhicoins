using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhicoin.Migrations
{
    public partial class AddVerificationIdTODb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "verification_id",
                table: "KYCVerificationTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verification_id",
                table: "KYCVerificationTokens");
        }
    }
}
