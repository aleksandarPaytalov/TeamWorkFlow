using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class TaskOperatorTableSeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1f3a56f1-cd8d-43d3-8af4-65ce516cc346", "AQAAAAEAACcQAAAAEJtu+hrt3N580HlBowYbDytPyd5VNGXODzrdnnxRN7DUSDaMaWj/KOiRdzgA1V9exg==", "2bee0aef-e08f-4d02-8f67-17e6a65f0ffa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8fc3edcd-98a4-4ac1-b5b3-84df5c9e5784", "AQAAAAEAACcQAAAAENJm57fo9d+L4i4UgAq7r8bu4hsvX9HV8HahTQLRG89859T9ABQ8v50aY0hFsalhtQ==", "a4c2e256-fbd3-4004-95e4-59d82f6f95dc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "36d252dd-34f7-459d-b304-17c1c8acde30", "AQAAAAEAACcQAAAAENzF4xpdhSD579op3RmeJ0RBsobRcALeBf3uIbVXRWIIt3CxKvygPsalurOd6DZe8g==", "17d9fd6d-f596-4c8c-9482-af3be346ecab" });

            migrationBuilder.InsertData(
                table: "TasksOperators",
                columns: new[] { "OperatorId", "TaskId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 6 },
                    { 2, 2 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TasksOperators",
                keyColumns: new[] { "OperatorId", "TaskId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TasksOperators",
                keyColumns: new[] { "OperatorId", "TaskId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "TasksOperators",
                keyColumns: new[] { "OperatorId", "TaskId" },
                keyValues: new object[] { 2, 2 });

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
    }
}
