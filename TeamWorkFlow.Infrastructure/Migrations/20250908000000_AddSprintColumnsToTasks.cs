using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddSprintColumnsToTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInSprint",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if task is included in current sprint");

            migrationBuilder.AddColumn<int>(
                name: "SprintOrder",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Order position in sprint for drag-and-drop functionality");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedStartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                comment: "Planned start date in sprint");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedEndDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                comment: "Planned completion date in sprint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInSprint",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SprintOrder",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PlannedEndDate",
                table: "Tasks");
        }
    }
}
