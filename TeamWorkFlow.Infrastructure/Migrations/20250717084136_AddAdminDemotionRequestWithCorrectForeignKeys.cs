using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddAdminDemotionRequestWithCorrectForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminDemotionRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDemotionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminDemotionRequests_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AdminDemotionRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminDemotionRequests_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimValue",
                value: "admin@test.local");

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimValue",
                value: "operator@test.local");

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimValue",
                value: "guest@test.local");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "07522802-e51b-4565-a9f8-d9096519ac59", "operator@test.local", "OPERATOR@TEST.LOCAL", "OPERATOR@TEST.LOCAL", "AQAAAAEAACcQAAAAEA25hdPGf2XkYuJpxCIupuqUlgz3ilC4vBAtKkIdJHus6DsTzBYfy3zA4aJ915SsLA==", "0420483e-3fc2-475d-a949-f0bdaad38a16", "operator@test.local" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "6d1d9e4b-2fa9-4a3d-8861-20000c98d645", "guest@test.local", "GUEST@TEST.LOCAL", "GUEST@TEST.LOCAL", "AQAAAAEAACcQAAAAEJK1ai3NgdOV3YlLKXMnLHXpRXIqTwKXZZV2fUWYrzi0sOnz+qd1UOmTcYhHBiLetw==", "8cd79661-cabb-4b29-befd-1ed8ee39a0f8", "guest@test.local" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "1062346f-496a-4189-a3be-4d8865911395", "admin@test.local", "ADMIN@TEST.LOCAL", "ADMIN@TEST.LOCAL", "AQAAAAEAACcQAAAAELnplwZqOIiTrvZgLqxSgJ/oN/Hgaa1qPC+z33fYp+nfatkHCDcm7iYMKyX/c7yhjA==", "2157c591-f150-4238-a0a3-d8c28742cb8f", "admin@test.local" });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "Capacity",
                value: 24);

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "Capacity",
                value: 24);

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "admin@test.local", "Test Admin", "+1234567890" });

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "operator@test.local", "Test Operator", "+1234567891" });

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "guest@test.local", "Test Guest", "+1234567892" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminDemotionRequests_ApprovedByUserId",
                table: "AdminDemotionRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminDemotionRequests_RequestedByUserId",
                table: "AdminDemotionRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminDemotionRequests_TargetUserId",
                table: "AdminDemotionRequests",
                column: "TargetUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminDemotionRequests");

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimValue",
                value: "ap.softuni@gmail.com");

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimValue",
                value: "jon.doe@softuni.bg");

            migrationBuilder.UpdateData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimValue",
                value: "jane.doe@softuni.bg");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "1f3a56f1-cd8d-43d3-8af4-65ce516cc346", "jon.doe@softuni.bg", "JON.DOE@SOFTUNI.BG", "JON.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAEJtu+hrt3N580HlBowYbDytPyd5VNGXODzrdnnxRN7DUSDaMaWj/KOiRdzgA1V9exg==", "2bee0aef-e08f-4d02-8f67-17e6a65f0ffa", "jon.doe@softuni.bg" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "8fc3edcd-98a4-4ac1-b5b3-84df5c9e5784", "jane.doe@softuni.bg", "JANE.DOE@SOFTUNI.BG", "JANE.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAENJm57fo9d+L4i4UgAq7r8bu4hsvX9HV8HahTQLRG89859T9ABQ8v50aY0hFsalhtQ==", "a4c2e256-fbd3-4004-95e4-59d82f6f95dc", "jane.doe@softuni.bg" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "36d252dd-34f7-459d-b304-17c1c8acde30", "ap.softuni@gmail.com", "AP.SOFTUNI@GMAIL.COM", "AP.SOFTUNI@GMAIL.COM", "AQAAAAEAACcQAAAAENzF4xpdhSD579op3RmeJ0RBsobRcALeBf3uIbVXRWIIt3CxKvygPsalurOd6DZe8g==", "17d9fd6d-f596-4c8c-9482-af3be346ecab", "ap.softuni@gmail.com" });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "Capacity",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "Capacity",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "ap.softuni@gmail.com", "Aleksandar Paytalov", "+359881234567" });

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "jon.doe@softuni.bg", "Jon Doe", "+359887654321" });

            migrationBuilder.UpdateData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "jane.doe@softuni.bg", "Jane Doe", "+359894567890" });
        }
    }
}
