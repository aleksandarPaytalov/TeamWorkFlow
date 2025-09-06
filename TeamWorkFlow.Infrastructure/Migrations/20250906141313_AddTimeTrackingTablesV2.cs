using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddTimeTrackingTablesV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskTimeEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Task time entry identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false, comment: "Task identifier"),
                    OperatorId = table.Column<int>(type: "int", nullable: false, comment: "Operator identifier"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Work session start time"),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Work session end time"),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false, comment: "Total duration of work session in minutes"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Optional notes about the work session"),
                    SessionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Type of work session (e.g., 'Development', 'Testing', 'Review')"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Timestamp when the time entry was created")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTimeEntries_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskTimeEntries_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Task time entry data model - tracks individual completed work sessions");

            migrationBuilder.CreateTable(
                name: "TaskTimeSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Task time session identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false, comment: "Task identifier"),
                    OperatorId = table.Column<int>(type: "int", nullable: false, comment: "Operator identifier"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Work session start time"),
                    LastPauseTime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Last time the session was paused"),
                    TotalPausedMinutes = table.Column<int>(type: "int", nullable: false, comment: "Total minutes the session has been paused"),
                    IsPaused = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the session is currently paused"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Current status of the session (e.g., 'Active', 'Paused', 'Completed')"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Timestamp when the session was created"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Timestamp when the session was last updated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTimeSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTimeSessions_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskTimeSessions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Task time session data model - tracks active work sessions in progress");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6c43743-a77b-454d-95fd-fb5d092b1383", "AQAAAAEAACcQAAAAEEyT150B0qFgVHfJz3POdQx+U4OMorxfc7fgpPwj1axnpte6FVECovBB+z+hkRpqGA==", "cf173120-fb95-4ae5-a3f1-70f5c223bb87" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6473961e-64b2-4948-88c9-4f4160b5f66e", "AQAAAAEAACcQAAAAEPwKMH+B2ooDYrLV/duqz793jklozzTl41zMvHiJGWuYhhs81xgIpxfFPWYnGjFlCg==", "6cacd335-6b2c-4e91-8172-660e7e8b3008" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e33335c-9d7f-45ea-9716-d8f25b868d5e", "AQAAAAEAACcQAAAAEJd31KmVpg7bO25eqEcrZoDHL58MQ9PeapB0fKVP44bltBWx/Enw+1KJ+Jmfm2lSeA==", "383e2e0a-b81b-4628-91af-5ed197cbca3e" });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeadLine", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeadLine", "StartDate" },
                values: new object[] { new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActualTime", "CompletedById", "EndDate", "StartDate" },
                values: new object[] { 224.0, "7bf9623c-54d9-45ba-84c6-52806dcee7bd", new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActualTime", "CompletedById", "DeadLine", "EndDate", "StartDate" },
                values: new object[] { 40.0, "cf41999b-9cad-4b75-977d-a2fdb3d02e77", new DateTime(2024, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });



            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeEntries_OperatorId",
                table: "TaskTimeEntries",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeEntries_TaskId",
                table: "TaskTimeEntries",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeSessions_OperatorId",
                table: "TaskTimeSessions",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeSessions_TaskId",
                table: "TaskTimeSessions",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTimeEntries");

            migrationBuilder.DropTable(
                name: "TaskTimeSessions");



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

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeadLine", "StartDate" },
                values: new object[] { new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeadLine", "StartDate" },
                values: new object[] { new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActualTime", "CompletedById", "EndDate", "StartDate" },
                values: new object[] { null, null, new DateTime(2024, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActualTime", "CompletedById", "DeadLine", "EndDate", "StartDate" },
                values: new object[] { null, null, new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
