using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShopSolution.Data.Migrations
{
    public partial class ChangeDataTypeOfFileSizeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "ProductImages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "3f781717-1b48-4335-8f9e-32bc2a325114");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "46286124-f7c9-420d-9315-8156b4a252e6", "AQAAAAEAACcQAAAAEFqGaplWZ0rAUL26Lhe0O1tVWdKBvVJ1m0lVt6QcNshNrjPSu5FPXVPX+stV0QvJgg==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 7, 22, 1, 28, 46, 295, DateTimeKind.Local).AddTicks(7169));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileSize",
                table: "ProductImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "0ce1ab00-b2ff-4438-b758-a12366f385f6");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ef7f4dab-0469-4197-9fed-472d2dbd03dd", "AQAAAAEAACcQAAAAEMEVSe8kMMzHQwisVSRTwGUg07Gc+MKtxtpd2yz5CYdjVUqtPJ93oHq62n/yi1Q0EA==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 7, 21, 22, 25, 26, 247, DateTimeKind.Local).AddTicks(2940));
        }
    }
}
