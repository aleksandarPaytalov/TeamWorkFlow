using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class SeedOperatorsIntoDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Operators",
                columns: new[] { "Id", "AvailabilityStatusId", "Capacity", "Email", "FullName", "IsActive", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 4, 8, "ap.softuni@gmail.com", "Aleksandar Paytalov", true, "+359881234567" },
                    { 2, 1, 4, "jon.doe@softuni.bg", "Jon Doe", true, "+359887654321" },
                    { 3, 2, 8, "jane.doe@softuni.bg", "Jane Doe", true, "+359894567890" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5380007e-957f-410f-a912-b4f6d37e0e64", "AQAAAAEAACcQAAAAEPq2dpkRcdeJj641v58aSD62K6mTkZn+A9eX1l4YWUtdcAnBpwguXaJxsmX7eXzeyw==", "e62f3f64-9faf-4fad-bdce-4aec2b155adb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f653c572-6c8c-48d6-ada6-02f2b03e082b", "AQAAAAEAACcQAAAAEAEvuauTGSvazqiPoVmCbn6pZkrqAadpF0CfOB0JBIDaSoeHWqSgXbw9jtd62s08fQ==", "a880a763-4a5d-47dc-ab84-b967bb4461c3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6fba670f-83f8-4074-9d9f-bf3a30f93035", "AQAAAAEAACcQAAAAEIgWIeeoslonZuuHuX4HadtHKzzE6BHTd02B2pK1G5T3JuJ+bqGhO58wFGR/k6RELA==", "91cd76c0-330d-4fe1-b982-1ad678ab39dc" });
        }
    }
}
