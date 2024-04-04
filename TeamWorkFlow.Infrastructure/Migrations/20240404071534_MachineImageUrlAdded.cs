using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class MachineImageUrlAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Machines",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "Machine picture");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Machines");
        }
    }
}
