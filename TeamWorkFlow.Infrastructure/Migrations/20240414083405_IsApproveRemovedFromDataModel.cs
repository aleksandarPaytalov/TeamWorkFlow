using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class IsApproveRemovedFromDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Operators");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0e69f5eb-fa82-4550-9090-d6e5b3dcd0fc", "AQAAAAEAACcQAAAAEGlioKHXb9fo8eFHS+a7NOo2hMMVrg2ZQXEu0s3+JNzoLt+HM22dSqNMZZH4jHfTJA==", "4cd6a9b3-86b9-4372-98bf-70b59e015a86" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "876f7332-34b0-4124-adab-1b66d5855dd2", "AQAAAAEAACcQAAAAEGvfoP7NSa8KsuiO7zDamflzwQdazPGPwYmzNA4EBlz1xoRAZ4Hc20W1aEby98iJjQ==", "190bcf7a-72f8-40eb-b6be-d0ffa49f4d90" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cae56427-a3da-4480-9b13-9116013de4a1", "AQAAAAEAACcQAAAAELsuJFGnr7DDGswazoHf/JDZRkqopMxen4/E56oUbtJE4NvceFB1wYzmAJKXuJXQEg==", "f380b92c-92da-4e6b-86da-5fb9d3522119" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
