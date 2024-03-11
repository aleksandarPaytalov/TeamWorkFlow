using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class FirstMigrationWithSeededData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Machine identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Machine name"),
                    Capacity = table.Column<int>(type: "int", nullable: false, comment: "Machine capacity"),
                    MaintenanceScheduleStartDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Machine maintenanceScheduleStartDate"),
                    MaintenanceScheduleEndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Machine maintenanceScheduleEndDate"),
                    CalibrationSchedule = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Machine calibration schedule"),
                    TotalMachineLoad = table.Column<double>(type: "float", nullable: false, comment: "Machine total load")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                },
                comment: "Machine db model");

            migrationBuilder.CreateTable(
                name: "OperatorAvailabilityStatusEnumerable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Operator identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, comment: "Availability status name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorAvailabilityStatusEnumerable", x => x.Id);
                },
                comment: "Operator availability status db model");

            migrationBuilder.CreateTable(
                name: "PartStatusEnumerable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "PartStatus identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, comment: "PartStatus name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartStatusEnumerable", x => x.Id);
                },
                comment: "Part status Db model");

            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Priority identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, comment: "Priority name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                },
                comment: "Priority data model");

            migrationBuilder.CreateTable(
                name: "ProjectStatusEnumerable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "ProjectStatus identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "ProjectStatus name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusEnumerable", x => x.Id);
                },
                comment: "ProjectStatus data model");

            migrationBuilder.CreateTable(
                name: "TaskStatusEnumerable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "TaskStatus identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "TaskStatus name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusEnumerable", x => x.Id);
                },
                comment: "TaskStatus data model");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Operator identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "First and Last name of the operator"),
                    AvailabilityStatusId = table.Column<int>(type: "int", nullable: false, comment: "Operator status identifier"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Operator phoneNumber"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Showing if the current operator is still working in the company"),
                    Capacity = table.Column<int>(type: "int", nullable: false, comment: "Operator working capacity in hours per day/shift")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operators_OperatorAvailabilityStatusEnumerable_AvailabilityStatusId",
                        column: x => x.AvailabilityStatusId,
                        principalTable: "OperatorAvailabilityStatusEnumerable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Operator DB model");

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Project identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "Project number"),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Project name"),
                    ProjectStatusId = table.Column<int>(type: "int", nullable: false, comment: "ProjectStatus identifier"),
                    ClientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Client name"),
                    Appliance = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Project appliance sector"),
                    TotalHoursSpent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectStatusEnumerable_ProjectStatusId",
                        column: x => x.ProjectStatusId,
                        principalTable: "ProjectStatusEnumerable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Project data model");

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Part identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Part name"),
                    PartArticleNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "Part article number"),
                    PartClientNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "Client article number for the current part"),
                    ToolNumber = table.Column<int>(type: "int", nullable: false, comment: "Part tool number"),
                    PartStatusId = table.Column<int>(type: "int", nullable: false, comment: "PartStatus identifier"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartModel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_PartStatusEnumerable_PartStatusId",
                        column: x => x.PartStatusId,
                        principalTable: "PartStatusEnumerable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Part Db model");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Task identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Task Name"),
                    Description = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false, comment: "Task description"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Task starting date"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "The date when the task is finished"),
                    TaskStatusId = table.Column<int>(type: "int", nullable: false, comment: "TaskStatus identifier"),
                    PriorityId = table.Column<int>(type: "int", nullable: false, comment: "Priority identifier"),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Task creator identifier"),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedTime = table.Column<int>(type: "int", nullable: false, comment: "Estimated time for the Task that is needed to be complete"),
                    Comment = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true, comment: "Comment for the current task"),
                    Attachment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Task attachments - files, drawings, documents, etc."),
                    MachineId = table.Column<int>(type: "int", nullable: false, comment: "Machine identifier"),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Priorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatusEnumerable_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalTable: "TaskStatusEnumerable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Task Db model");

            migrationBuilder.CreateTable(
                name: "TasksOperators",
                columns: table => new
                {
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksOperators", x => new { x.OperatorId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TasksOperators_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TasksOperators_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "TaskOperator data model");

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CalibrationSchedule", "Capacity", "MaintenanceScheduleEndDate", "MaintenanceScheduleStartDate", "Name", "TotalMachineLoad" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2615), 20, null, null, "Zeiss Contura", 0.0 },
                    { 2, new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2617), 20, null, null, "Zeiss O-inspect", 0.0 },
                    { 10, new DateTime(2024, 3, 11, 17, 46, 48, 772, DateTimeKind.Local).AddTicks(2618), 20, null, null, "Zeiss Metrotom", 0.0 }
                });

            migrationBuilder.InsertData(
                table: "OperatorAvailabilityStatusEnumerable",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "at work" },
                    { 2, "in sick leave" },
                    { 3, "on vacation" },
                    { 4, "on training" }
                });

            migrationBuilder.InsertData(
                table: "PartStatusEnumerable",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "released" },
                    { 2, "not released" },
                    { 3, "Conditional released" }
                });

            migrationBuilder.InsertData(
                table: "Priorities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "low" },
                    { 2, "normal" },
                    { 3, "high" }
                });

            migrationBuilder.InsertData(
                table: "ProjectStatusEnumerable",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "In production" },
                    { 2, "In development" },
                    { 3, "in ACL" }
                });

            migrationBuilder.InsertData(
                table: "TaskStatusEnumerable",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "open" },
                    { 2, "in progress" },
                    { 3, "finished" },
                    { 4, "canceled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_AvailabilityStatusId",
                table: "Operators",
                column: "AvailabilityStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartStatusId",
                table: "Parts",
                column: "PartStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_ProjectId",
                table: "Parts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStatusId",
                table: "Projects",
                column: "ProjectStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatorId",
                table: "Tasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_MachineId",
                table: "Tasks",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TasksOperators_TaskId",
                table: "TasksOperators",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "TasksOperators");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PartStatusEnumerable");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "OperatorAvailabilityStatusEnumerable");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "TaskStatusEnumerable");

            migrationBuilder.DropTable(
                name: "ProjectStatusEnumerable");
        }
    }
}
