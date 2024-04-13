using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddUserIdAndUserRelationToOperatorTableInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Operators",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                comment: "User identifier");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8dbadaf7-bae1-4de0-ac15-755fd64ee967", "AQAAAAEAACcQAAAAENGfodwkCH6AISI06P+Bqy8KL+f1CMHtVcPUr+Y/N6Okrb6Y4/u4dQ1jY24YKR2lNg==", "91820608-819e-4585-a31e-bd0b7da263dc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "042d1d00-bf98-4b06-becc-fac87b2636a1", "AQAAAAEAACcQAAAAEIQrgPUe9kMHqy2zVaan7ZZo5Tl6VYVB9vHmBNSLrfJ8+7PpTZ5l+su+y1JSmLVLvw==", "b0620019-9ba2-448a-b82e-3a8d78464f8e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c895617-14d4-4e15-9ab7-280de4b3703c", "AQAAAAEAACcQAAAAEG9GjPAQ/J5TWvs9DhzGTOXZ+NqjdNYmnI8VEqGwrqemZlZXkdO85RG9t8wkeBpqIg==", "b422b79a-43eb-4d8a-8edf-7c99c6bcd470" });

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "cf41999b-9cad-4b75-977d-a2fdb3d02e77");

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: "7bf9623c-54d9-45ba-84c6-52806dcee7bd");

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: "b806eee6-2ceb-4956-9643-e2e2e82289d2");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_UserId",
                table: "Operators",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operators_AspNetUsers_UserId",
                table: "Operators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operators_AspNetUsers_UserId",
                table: "Operators");

            migrationBuilder.DropIndex(
                name: "IX_Operators_UserId",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Operators");

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
        }
    }
}
