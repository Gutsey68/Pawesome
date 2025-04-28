using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawesome.Migrations
{
    /// <inheritdoc />
    public partial class Additionalinfoadvert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "Adverts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "Adverts");
        }
    }
}
