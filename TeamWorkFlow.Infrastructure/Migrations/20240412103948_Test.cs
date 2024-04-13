using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "acdf58cd-9793-4207-98a2-7e1e463cc5d3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f35c92a-24c7-4b40-898e-b7bbf5d060f3", "AQAAAAEAACcQAAAAEMCr2gXmxugchgOw1PcX3RvYEs2h470/1wiRj7wawLCjgZD2xKOZVpEh1mL69BGRBg==", "6928dfad-97c9-4d0b-8d2d-133fd4579eb9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "6eb17026-e703-4197-9ebd-3577fb8d51fc", "ap.softuni9@gmail.com", "AP.SOFTUNI9@GMAIL.COM", "AP.SOFTUNI9@GMAIL.COM", "AQAAAAEAACcQAAAAEDHif1VXYLZy3bU96D+65c/WChgE2FHD4CLtP1pRC/S8PjAyUwy3TyzFauou2C3fzw==", "cadcbae7-c1ae-449b-bd53-b5a433059e5b", "ap.softuni9@gmail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe0a41ef-1ea3-4fe5-8a78-8adfe5f020ff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d34feabd-14dc-44ad-91bf-abc542fa21cd", "AQAAAAEAACcQAAAAEAZHaMufHdSSD9ebp59ISynfrHaYaDOz/5B3LBUjwCi1snHQJWkFvd2mkIqSLgVrLg==", "8d94a42e-3312-41e5-b51a-0d23c7053f9c" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "acdf58cd-9793-4207-98a2-7e1e463cc5d3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eca5787e-5190-4ef1-b5c3-a810dfda1a88", "AQAAAAEAACcQAAAAECl6l/1wglhrdUFCIg/fYNZEgRHiS6Id7EouqZaXLQTaN459sBB9z21j/MkOo65DpQ==", "b4c94f35-287f-4c6b-9b46-1b4a0b4bc19f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e733b261-0a9c-45eb-ad97-cc611c83e2dd",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "5e1bfd87-0345-4df2-a890-c5c3fe11bec3", "ap.softuni@gmail.com", "AP.SOFTUNI@GMAIL.COM", "AP.SOFTUNI@GMAIL.COM", "AQAAAAEAACcQAAAAEI0Mc26AohQl+PY/KtT6CR26wPJC99Sg6kS2YiD1MAWhIIG8ZvKnwTucWqqdLfuYbA==", "d25aac8c-0d0e-43fa-b762-3bc1d46e29d2", "ap.softuni@gmail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe0a41ef-1ea3-4fe5-8a78-8adfe5f020ff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d298171-624f-44c8-b885-73e6ba7463d7", "AQAAAAEAACcQAAAAEAuJbX7AZmp7w5/AOSGEabwglhjhTPECdpOjUpJ2XBXnCT+Sk+wWHtfaDHajnrn/cA==", "9ce794e6-3df4-48c0-87ed-050377fae172" });
        }
    }
}
