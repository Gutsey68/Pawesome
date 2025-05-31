using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawesome.Migrations
{
    /// <inheritdoc />
    public partial class Stripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStripeOnboardingCompleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStripeOnboardingCompleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "AspNetUsers");
        }
    }
}
