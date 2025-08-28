using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddTasksNavigationPropertyToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0b36bdf5-8a96-45ae-896c-0dd52a67c9b4", "AQAAAAEAACcQAAAAEHmLZi9IRcTSuO3T6xPJdnnKYUzbsnYeJz8IN6NZXU91TX3SS/x9enCbt5F33UErdQ==", "58b95bbd-4f48-4245-b16f-eb61dfb67a49" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d0aa300b-87d0-46a1-aa6c-ac2353f9dfc4", "AQAAAAEAACcQAAAAELp9f6sHH0mTXRMyMWXa7ubotJwfNPL8XtHzmogn9hJxSoboiVGnFv0SH9QcCVns9Q==", "3ab6ee92-ca9e-4527-8792-96f3b215ae39" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5aa9d1a2-78c1-408b-89aa-e2aacea8f748", "AQAAAAEAACcQAAAAEH/o1Jzm6MF9qDnPmtkZUaN9IKCjhWnrQ8GCbAXgpwk97Kc1bQr0TVXgoDm5YPBMDQ==", "6c1b9440-a7d6-4d00-8eeb-0860427cbd9a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "07522802-e51b-4565-a9f8-d9096519ac59", "AQAAAAEAACcQAAAAEA25hdPGf2XkYuJpxCIupuqUlgz3ilC4vBAtKkIdJHus6DsTzBYfy3zA4aJ915SsLA==", "0420483e-3fc2-475d-a949-f0bdaad38a16" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6d1d9e4b-2fa9-4a3d-8861-20000c98d645", "AQAAAAEAACcQAAAAEJK1ai3NgdOV3YlLKXMnLHXpRXIqTwKXZZV2fUWYrzi0sOnz+qd1UOmTcYhHBiLetw==", "8cd79661-cabb-4b29-befd-1ed8ee39a0f8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1062346f-496a-4189-a3be-4d8865911395", "AQAAAAEAACcQAAAAELnplwZqOIiTrvZgLqxSgJ/oN/Hgaa1qPC+z33fYp+nfatkHCDcm7iYMKyX/c7yhjA==", "2157c591-f150-4238-a0a3-d8c28742cb8f" });
        }
    }
}
