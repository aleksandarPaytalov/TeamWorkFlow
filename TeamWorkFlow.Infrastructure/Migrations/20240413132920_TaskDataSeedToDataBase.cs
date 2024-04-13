using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class TaskDataSeedToDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Attachment", "Comment", "CreatorId", "DeadLine", "Description", "EndDate", "EstimatedTime", "MachineId", "Name", "PriorityId", "ProjectId", "StartDate", "TaskStatusId" },
                values: new object[,]
                {
                    { 1, null, null, "cf41999b-9cad-4b75-977d-a2fdb3d02e77", new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOP dimensional report for phase 1 (T0) - samples from the tool maker should arrive in Calendar week 48.", null, 25, 1, "Housing Front Panel - LOP.", 2, 2, new DateTime(2023, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, null, null, "cf41999b-9cad-4b75-977d-a2fdb3d02e77", new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "PPAP level 3", null, 32, 2, "Housing Klima - PPAP", 2, 3, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, null, null, "cf41999b-9cad-4b75-977d-a2fdb3d02e77", new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Full PPAP documents need to be created and prepared for sending to customer no late than 07.07.2024.", null, 32, null, "Housing D8 - PPAP", 2, 2, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, null, null, "cf41999b-9cad-4b75-977d-a2fdb3d02e77", null, "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine", null, 8, null, "BMW Back Panel - Sample order no. 987", 2, 1, new DateTime(2024, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 5, null, null, "7bf9623c-54d9-45ba-84c6-52806dcee7bd", null, "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine", new DateTime(2024, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 2, "BMW Front panel - Sample order No. 954", 1, 1, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 6, null, null, "7bf9623c-54d9-45ba-84c6-52806dcee7bd", new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "PPAP documents level 3 must be performed. Note: Deviations on dimensions 10 and 150 have been accepted from the customer. Drawing will be adjusted with next PPAP revision", new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, 3, "Housing Klima module V6 - PPAP", 3, 3, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8beca5d7-6112-4308-8bda-eaa2d6731910", "AQAAAAEAACcQAAAAEJHgp2mIeqwwwzCrdTONGHXwT067/0dXfMU9rEUTRWVDkGc00kE1LDBxHS1rA6P16A==", "48ce4be3-e2cc-4505-b776-2235d4d91d5a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b457b5b-0a03-45c7-a3f3-6421cc601007", "AQAAAAEAACcQAAAAEI8NMymfamuGj/4qzADJ7xqMPZOMwUM7jjiMBDGJaYwasVoScBPv7uZ8fHbY7DZbNA==", "510f9fd3-e8cd-4348-bfcf-8c934a9b3233" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd3b5b27-26e6-4807-8f89-9f31ffffc2c5", "AQAAAAEAACcQAAAAEM+PpOZLjI+c45Nx28nhNpiSPDnwICIYYFvpADD/GNikzrSDKnJyLKhKPrTNvlg2DA==", "d066a5c8-37f9-405f-b638-3776c8c92970" });
        }
    }
}
