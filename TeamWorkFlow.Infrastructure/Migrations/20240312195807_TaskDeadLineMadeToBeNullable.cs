using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class TaskDeadLineMadeToBeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeadLine",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeadLine",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 18, 18, 54, 238, DateTimeKind.Local).AddTicks(5403));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 18, 18, 54, 238, DateTimeKind.Local).AddTicks(5405));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 18, 18, 54, 238, DateTimeKind.Local).AddTicks(5406));
        }
    }
}
