using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class SecondMigrationAddedAdditionalPropertiesInTablesTaskAndPart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Attachments",
                table: "Tasks",
                newName: "Attachment");

            migrationBuilder.AddColumn<string>(
                name: "PartModel",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ProjectStatusEnumerable",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In development");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartModel",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "Tasks",
                newName: "Attachments");

            migrationBuilder.UpdateData(
                table: "ProjectStatusEnumerable",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In Production");
        }
    }
}
