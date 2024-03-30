using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class MachineIdInTaskModelAllowedToBeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Machines_MachineId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "Tasks",
                type: "int",
                nullable: true,
                comment: "Machine identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Machine identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Machines_MachineId",
                table: "Tasks",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Machines_MachineId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Machine identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Machine identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Machines_MachineId",
                table: "Tasks",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
