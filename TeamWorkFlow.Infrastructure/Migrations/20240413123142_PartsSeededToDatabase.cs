using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class PartsSeededToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5380007e-957f-410f-a912-b4f6d37e0e64", "AQAAAAEAACcQAAAAEPq2dpkRcdeJj641v58aSD62K6mTkZn+A9eX1l4YWUtdcAnBpwguXaJxsmX7eXzeyw==", "e62f3f64-9faf-4fad-bdce-4aec2b155adb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f653c572-6c8c-48d6-ada6-02f2b03e082b", "AQAAAAEAACcQAAAAEAEvuauTGSvazqiPoVmCbn6pZkrqAadpF0CfOB0JBIDaSoeHWqSgXbw9jtd62s08fQ==", "a880a763-4a5d-47dc-ab84-b967bb4461c3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6fba670f-83f8-4074-9d9f-bf3a30f93035", "AQAAAAEAACcQAAAAEIgWIeeoslonZuuHuX4HadtHKzzE6BHTd02B2pK1G5T3JuJ+bqGhO58wFGR/k6RELA==", "91cd76c0-330d-4fe1-b982-1ad678ab39dc" });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "ImageUrl", "Name", "PartArticleNumber", "PartClientNumber", "PartModel", "PartStatusId", "ProjectId", "ToolNumber" },
                values: new object[,]
                {
                    { 1, "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_AudiA1.jpg", "VW Housing Front D9", "2.4.100.501", "252.166-15", "252.166-15_0B_VW Housing Front D9", 2, 2, 9055 },
                    { 2, "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358", "VW Housing D8", "2.4.100.502", "252.167-00", "252.167-00_0D_VW Housing D8", 2, 2, 3418 },
                    { 3, "https://wodofogdr.com/cdn/shop/products/GDR-MBT-823287-2_grande.jpg?v=1626163358", "Audi Housing A5 X-line", "2.4.100.605", "312.205-11", "334.255-10_0E_Audi Housing A5 X-line", 1, 2, 3459 },
                    { 4, "https://www.bhtc.com/media/pages/produkte/fahrzeugklimatisierung/bmw-klimabediengerat/3086657772-1542633776/bmw_klimabediengeraet_gkl.png", "Toyota Housing F5", "2.4.202.333", "212.200-00", "212.200-00_0B_Toyota Housing F5", 3, 3, 5533 },
                    { 5, "https://conti-engineering.com/wp-content/uploads/2020/09/climatecontrol_beitrag.jpg", "BMW Front-Back Panels X5", "2.3.105.603", "212.200-11", "212.200-11_0E_BMW Front-Back Panels X5", 3, 1, 3360 },
                    { 6, "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_FordFocus.jpg", "VW Tuareg Housing panel G5", "2.4.305.777", "431.222-07", "431.222-07_0A_VW Tuareg Housing panel G5", 1, 2, 2515 },
                    { 7, "https://www.preh.com/fileadmin/templates/website/media/images/Produkte/Car_HMI/Climate_Control/Preh_Produkte_Climate_Control_AudiR8.jpg", "Toyota Aventis Housing Klima module V6", "2.4.105.589", "305.201-11", "305.201-11_0B_Toyota Aventis Housing Klima module V6", 1, 3, 9999 },
                    { 8, "https://autoprotoway.com/wp-content/uploads/2022/09/precision-automotive-lighting-parts.jpg", "VW Light Conductor Front Panel", "2.4.222.777", "213.891-22", "213.891-22_0T_VW Light Conductor Front Panel", 1, 2, 9995 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                table: "Parts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38ff4114-aaaf-4ab8-8698-cf5d76c7b379", "AQAAAAEAACcQAAAAEEdAAnx2snH8vvyuOBVgIMc5+nv6LwK+fbasH1WlYdF8PLFaOQsg4RDb3Kf9vGrz5w==", "bb6c387e-d76f-4ab4-b319-9e4bdb94491d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6a7245c5-88c9-410e-b5a5-e3cdb7ce21bb", "AQAAAAEAACcQAAAAEOgH9Oi1x17iitPp6iYq4dC0pw8UVqhMPMxdEsN96pVKpV+HSeK3MJwiQH7gsdd9kg==", "639c9c13-3e8a-46dd-aa7e-aa802f2c92c1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "11f1fc9a-45d7-4032-8ffc-22a47857059a", "AQAAAAEAACcQAAAAECo5InvtDuXGzzvttemHoD4j97GTBmVx3gKC53Ll+rX4BpfORp8iE8Pima32h5qWUg==", "f7786638-4c92-4412-943c-67de8fac5e96" });
        }
    }
}
