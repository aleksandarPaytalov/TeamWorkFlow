using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class TaskOperatorsAndPartsSeededInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
