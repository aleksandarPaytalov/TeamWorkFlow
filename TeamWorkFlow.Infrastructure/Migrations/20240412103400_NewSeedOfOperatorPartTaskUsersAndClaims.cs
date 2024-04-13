using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class NewSeedOfOperatorPartTaskUsersAndClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "acdf58cd-9793-4207-98a2-7e1e463cc5d3", 0, "eca5787e-5190-4ef1-b5c3-a810dfda1a88", "jane.doe@softuni.bg", true, "Jane", "Doe", false, null, "JANE.DOE@SOFTUNI.BG", "JANE.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAECl6l/1wglhrdUFCIg/fYNZEgRHiS6Id7EouqZaXLQTaN459sBB9z21j/MkOo65DpQ==", "+359894567890", false, "b4c94f35-287f-4c6b-9b46-1b4a0b4bc19f", false, "jane.doe@softuni.bg" },
                    { "e733b261-0a9c-45eb-ad97-cc611c83e2dd", 0, "5e1bfd87-0345-4df2-a890-c5c3fe11bec3", "ap.softuni@gmail.com", true, "Aleksandar", "Paytalov", false, null, "AP.SOFTUNI@GMAIL.COM", "AP.SOFTUNI@GMAIL.COM", "AQAAAAEAACcQAAAAEI0Mc26AohQl+PY/KtT6CR26wPJC99Sg6kS2YiD1MAWhIIG8ZvKnwTucWqqdLfuYbA==", "+359881234567", false, "d25aac8c-0d0e-43fa-b762-3bc1d46e29d2", false, "ap.softuni@gmail.com" },
                    { "fe0a41ef-1ea3-4fe5-8a78-8adfe5f020ff", 0, "7d298171-624f-44c8-b885-73e6ba7463d7", "jon.doe@softuni.bg", true, "Jon", "Doe", false, null, "JON.DOE@SOFTUNI.BG", "JON.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAEAuJbX7AZmp7w5/AOSGEabwglhjhTPECdpOjUpJ2XBXnCT+Sk+wWHtfaDHajnrn/cA==", "+359887654321", false, "9ce794e6-3df4-48c0-87ed-050377fae172", false, "jon.doe@softuni.bg" }
                });

            migrationBuilder.InsertData(
                table: "Operators",
                columns: new[] { "Id", "AvailabilityStatusId", "Capacity", "Email", "FullName", "IsActive", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 4, 8, "ap.softuni@gmail.com", "Aleksandar Paytalov", true, "+359881234567" },
                    { 2, 1, 4, "jon.doe@softuni.bg", "Jon Doe", true, "+359887654321" },
                    { 3, 2, 8, "jane.doe@softuni.bg", "Jane Doe", false, "+359894567890" }
                });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "ImageUrl", "Name", "PartArticleNumber", "PartClientNumber", "PartModel", "PartStatusId", "ProjectId", "ToolNumber" },
                values: new object[,]
                {
                    { 1, "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_AudiA1.jpg", "VW Housing Front D9", "2.4.100.501", "252.166-15", "252.166-15_0B_VW Housing Front D9", 2, 2, 9055 },
                    { 2, "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358", "VW Housing D8", "2.4.100.502", "252.167-00", "252.167-00_0D_VW Housing D8", 2, 2, 3418 },
                    { 3, "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358", "Audi Housing A5 X-line", "2.4.100.605", "312.205-11", "334.255-10_0E_Audi Housing A5 X-line", 1, 2, 3459 },
                    { 4, "https://www.bhtc.com/media/pages/produkte/fahrzeugklimatisierung/bmw-klimabediengerat/3086657772-1542633776/bmw_klimabediengeraet_gkl.png", "Toyota Housing F5", "2.4.202.333", "212.200-00", "212.200-00_0B_Toyota Housing F5", 3, 3, 5533 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "user:FirstAndSecondName", "Aleksandar Paytalov", "e733b261-0a9c-45eb-ad97-cc611c83e2dd" },
                    { 2, "user:FirstAndSecondName", "Jon Doe", "fe0a41ef-1ea3-4fe5-8a78-8adfe5f020ff" },
                    { 3, "user:FirstAndSecondName", "Jane Doe", "acdf58cd-9793-4207-98a2-7e1e463cc5d3" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Attachment", "Comment", "CreatorId", "DeadLine", "Description", "EndDate", "EstimatedTime", "MachineId", "Name", "PriorityId", "ProjectId", "StartDate", "TaskStatusId" },
                values: new object[,]
                {
                    { 4, null, null, "e733b261-0a9c-45eb-ad97-cc611c83e2dd", new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOP dimensional report for phase 1 (T0) - samples from the tool maker should arrive in Calendar week 48.", null, 2, 1, "Housing Front Panel - LOP.", 2, 2, new DateTime(2023, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 15, null, null, "e733b261-0a9c-45eb-ad97-cc611c83e2dd", new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "PPAP level 3", null, 0, null, "Housing Klima - PPAP", 2, 3, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 17, null, null, "e733b261-0a9c-45eb-ad97-cc611c83e2dd", new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Full PPAP documents need to be created and prepared for sending to customer no late than 07.07.2024.", null, 0, null, "Housing D8 - PPAP", 2, 2, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 18, null, null, "e733b261-0a9c-45eb-ad97-cc611c83e2dd", null, "Validation of the part on another production machine. Full dimensional report of 5 shots from the new machine. Results must be compared with measurements of the part from the serial (validated) production machine", null, 0, null, "BMW Back Panel - Sample order no. 987", 2, 1, new DateTime(2024, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 3);

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

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "acdf58cd-9793-4207-98a2-7e1e463cc5d3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e733b261-0a9c-45eb-ad97-cc611c83e2dd");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe0a41ef-1ea3-4fe5-8a78-8adfe5f020ff");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
