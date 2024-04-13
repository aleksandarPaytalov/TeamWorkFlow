using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class UsersSeededInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "7bf9623c-54d9-45ba-84c6-52806dcee7bd", 0, "38ff4114-aaaf-4ab8-8698-cf5d76c7b379", "jon.doe@softuni.bg", false, false, null, "JON.DOE@SOFTUNI.BG", "JON.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAEEdAAnx2snH8vvyuOBVgIMc5+nv6LwK+fbasH1WlYdF8PLFaOQsg4RDb3Kf9vGrz5w==", null, false, "bb6c387e-d76f-4ab4-b319-9e4bdb94491d", false, "jon.doe@softuni.bg" },
                    { "b806eee6-2ceb-4956-9643-e2e2e82289d2", 0, "6a7245c5-88c9-410e-b5a5-e3cdb7ce21bb", "jane.doe@softuni.bg", false, false, null, "JANE.DOE@SOFTUNI.BG", "JANE.DOE@SOFTUNI.BG", "AQAAAAEAACcQAAAAEOgH9Oi1x17iitPp6iYq4dC0pw8UVqhMPMxdEsN96pVKpV+HSeK3MJwiQH7gsdd9kg==", null, false, "639c9c13-3e8a-46dd-aa7e-aa802f2c92c1", false, "jane.doe@softuni.bg" },
                    { "cf41999b-9cad-4b75-977d-a2fdb3d02e77", 0, "11f1fc9a-45d7-4032-8ffc-22a47857059a", "ap.softuni@gmail.com", false, false, null, "AP.SOFTUNI@GMAIL.COM", "AP.SOFTUNI@GMAIL.COM", "AQAAAAEAACcQAAAAECo5InvtDuXGzzvttemHoD4j97GTBmVx3gKC53Ll+rX4BpfORp8iE8Pima32h5qWUg==", null, false, "f7786638-4c92-4412-943c-67de8fac5e96", false, "ap.softuni@gmail.com" }
                });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://www.researchgate.net/profile/Nermina_Zaimovic-Uzunovic2/publication/343880067/figure/fig2/AS:928740968255491@1598440510374/Measurement-of-the-top-surface-Fig4-CMM-Zeiss-Contura-G2_Q320.jpg");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://www.qpluslabs.com/wp-content/uploads/2019/11/Zeiss-O-Inspect-863-475px.jpg");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://i0.wp.com/metrology.news/wp-content/uploads/2023/02/ZEISS-METROTOM-1.jpg?resize=450%2C404");

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "userName", "ap.softuni@gmail.com", "cf41999b-9cad-4b75-977d-a2fdb3d02e77" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 2, "userName", "jon.doe@softuni.bg", "7bf9623c-54d9-45ba-84c6-52806dcee7bd" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 3, "userName", "jane.doe@softuni.bg", "b806eee6-2ceb-4956-9643-e2e2e82289d2" });
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
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "");
        }
    }
}
