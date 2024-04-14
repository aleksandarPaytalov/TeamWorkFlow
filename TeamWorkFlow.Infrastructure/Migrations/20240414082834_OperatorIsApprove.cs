using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class OperatorIsApprove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Operators",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Checker if the User is approved as operator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e71ab459-5a04-4863-a711-1889829057e7", "AQAAAAEAACcQAAAAEBiQdBlpTXytHpKVmQvyKTzBelYgcRBJsGrVGp62MRroktUBLidZ2E6d8kwbZBaIjA==", "55087060-019c-4d78-b949-b543625e5df6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "75abd990-7935-4331-9e09-a7db3a09ab8c", "AQAAAAEAACcQAAAAELiAONgtUCEx5b+kLE6MWEdDCij83NzO5gGJP39FOrrXirOC4YjUP9u/z75bxW3NIQ==", "e98fcd54-8222-42f0-b620-0dce142e20a1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "03808e8b-ef90-4ab7-9ba3-4a0681d01be8", "AQAAAAEAACcQAAAAEJcAkRvu8BwlD9svesTD7+pXlCfFqLXqLMI8aKAW4Xtt4iBtQrwJLS9CL7SEs7MRCA==", "68ebe27c-646d-4b05-afa2-6a377596b55b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Operators");

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
        }
    }
}
