using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class PrimaryKeyOfMachineMetrotomCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 17, 50, 27, 75, DateTimeKind.Local).AddTicks(82));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 17, 50, 27, 75, DateTimeKind.Local).AddTicks(84));

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CalibrationSchedule", "Capacity", "MaintenanceScheduleEndDate", "MaintenanceScheduleStartDate", "Name", "TotalMachineLoad" },
                values: new object[] { 3, new DateTime(2024, 3, 11, 17, 50, 27, 75, DateTimeKind.Local).AddTicks(86), 20, null, null, "Zeiss Metrotom", 0.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2615));

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2617));

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CalibrationSchedule", "Capacity", "MaintenanceScheduleEndDate", "MaintenanceScheduleStartDate", "Name", "TotalMachineLoad" },
                values: new object[] { 10, new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2618), 20, null, null, "Zeiss Metrotom", 0.0 });
        }
    }
}
