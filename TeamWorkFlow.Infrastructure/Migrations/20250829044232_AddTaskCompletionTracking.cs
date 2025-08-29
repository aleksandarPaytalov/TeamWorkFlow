using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddTaskCompletionTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualTime",
                table: "Tasks",
                type: "float",
                nullable: true,
                comment: "Actual time spent on the task in hours (calculated from start and end dates)");

            migrationBuilder.AddColumn<string>(
                name: "CompletedById",
                table: "Tasks",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                comment: "User identifier who completed the task");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51a70398-716a-4947-81f7-fd4eb7bacd05", "AQAAAAEAACcQAAAAEFu72IEgOXxoDsfnqeuiEBMrKd5NJXych6f4FV5Jclfa7OwJhjUDFHN5d8NReYBI6Q==", "581b66fc-aa49-4325-b66b-ef5b40024bad" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "28570edd-c236-444e-8074-893a2e75052e", "AQAAAAEAACcQAAAAEE0EXl9nEGfVMnf4uWk1qEwKjiPHdkoJlfnbUCJI2ZwJavUu3Kv6KSUVZnV2csibIw==", "e716c82e-fb6b-44b0-81b2-90ae27c507f1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5fa46574-3042-4bf4-af01-b74191c03e69", "AQAAAAEAACcQAAAAEBVSWm7bMU/MUq+7GDPQbRYATVnRlbiJ5wNoENNKJHKfFxUUkSG4WCwePP8pBvIIwA==", "756e1e41-c49f-4df8-b6b2-49ffd4cdc26b" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CompletedById",
                table: "Tasks",
                column: "CompletedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_CompletedById",
                table: "Tasks",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_CompletedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CompletedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ActualTime",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CompletedById",
                table: "Tasks");

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
    }
}
