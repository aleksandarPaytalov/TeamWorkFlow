using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddThreeMoreMachineToDatabaseSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "062f86d9-8e6b-4faa-9481-ee9bf011df9a", "AQAAAAEAACcQAAAAEAF6MhR8xsHOmQ5rtR3pAk4Ssm7GOeGw9GB6nRMFtcuqpQIUFV+Rd6VkcKaa7sTeFw==", "ef45f4ce-9f34-46ca-9d0e-3833ce4a7c2f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d9d19e80-610f-44db-ba6f-0ba6b4b4b2c9", "AQAAAAEAACcQAAAAEE9yVdjVGNdo9Q5lF5lxyD51yalW+rNN3sIwvoiK+/6vNUd7xwsimNWpeX6JwD82KA==", "341fba96-2787-4380-900c-fd0b81ff8c94" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6cf4375-e30e-4a8a-8b4c-f0fb723a7a57", "AQAAAAEAACcQAAAAEJ3aW/wj+0gRkMMP18NKfscyjbixAMok0gn6+IB76KE1o8X1wHRQ8TLou2sPM7djFw==", "73af7f97-6410-4d5b-93a4-f057c89712c3" });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CalibrationSchedule", "Capacity", "ImageUrl", "IsCalibrated", "MaintenanceScheduleEndDate", "MaintenanceScheduleStartDate", "Name", "TotalMachineLoad" },
                values: new object[,]
                {
                    { 4, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, "https://www.zeiss.com/content/dam/metrology/products/systems/ct/bosello-new/bosello-sre-max.jpg", true, null, null, "Zeiss X-ray", 0.0 },
                    { 5, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, "https://measuremetrology.com/wp-content/uploads/2023/03/mitutoyobrightapex504.png", true, null, null, "Mitutoyo Scan", 0.0 },
                    { 6, new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, "https://www.micro-shop.zeiss.com/data/image/shop-catalog-system/group_6038.jpg", true, null, null, "Zeiss Microscope E9000", 0.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be01fd79-8cc2-49e1-bdc2-806a96040b4d", "AQAAAAEAACcQAAAAEONE7zqxPJrpoB1AYHG835t6l5OE8gFsEm8EkzHKNz0BFZPTGzfUXjjZ6ReiIPGuAA==", "e1e40623-dc0b-4a74-ab39-1c735b5983e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22e965b2-ab26-4860-9364-5d22b6b060d6", "AQAAAAEAACcQAAAAEMtVCYdTZKs18tM77bcCIW10hWyZ+btQY+ShFJc6E5VVoztzBasdLD0VTMBW1xFqBQ==", "d3f4436a-425f-4121-9495-8057809f3186" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b3282daa-bb2b-4fc6-915e-b18a3bf282a9", "AQAAAAEAACcQAAAAEOPOFYivPD3k8ZAkkPON867iqZ95CIywk+nAk0h6g98bx24PZWNUjOopY+h3qpHS0A==", "a0fb7326-87f4-46a9-959f-22ac22eb787f" });
        }
    }
}
