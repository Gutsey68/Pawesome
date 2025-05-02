using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawesome.Migrations
{
    /// <inheritdoc />
    public partial class Reports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports");

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "Reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Reports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TargetId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId1",
                table: "Reports",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UserId1",
                table: "Reports",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UserId1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_UserId1",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
