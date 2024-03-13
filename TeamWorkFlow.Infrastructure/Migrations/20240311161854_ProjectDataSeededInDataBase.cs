using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class ProjectDataSeededInDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Appliance", "ClientName", "ProjectName", "ProjectNumber", "ProjectStatusId", "TotalHoursSpent" },
                values: new object[,]
                {
                    { 1, "Automotive industry", "Bmw", "BMW Housing Gx9", "249100", 1, 50 },
                    { 2, "Automotive industry", "Vw", "Vw Tuareg Front panel ", "249200", 2, 20 },
                    { 3, "Automotive industry", "Toyota", "Toyota Climatic module X5", "249300", 3, 41 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

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

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "CalibrationSchedule",
                value: new DateTime(2024, 3, 11, 17, 50, 27, 75, DateTimeKind.Local).AddTicks(86));
        }
    }
}
