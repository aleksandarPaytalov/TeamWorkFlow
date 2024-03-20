using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddConstraintsForOperatorCapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstimatedTime",
                table: "Tasks",
                type: "int",
                nullable: false,
                comment: "Estimated time for the Task that is needed to be complete - in hours",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Estimated time for the Task that is needed to be complete");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstimatedTime",
                table: "Tasks",
                type: "int",
                nullable: false,
                comment: "Estimated time for the Task that is needed to be complete",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Estimated time for the Task that is needed to be complete - in hours");
        }
    }
}
