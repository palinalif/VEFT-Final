using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShroomCity.API.Migrations
{
    /// <inheritdoc />
    public partial class Fixednullvalueinattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_Users_RegisteredById",
                table: "Attributes");

            migrationBuilder.AlterColumn<int>(
                name: "RegisteredById",
                table: "Attributes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Users_RegisteredById",
                table: "Attributes",
                column: "RegisteredById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_Users_RegisteredById",
                table: "Attributes");

            migrationBuilder.AlterColumn<int>(
                name: "RegisteredById",
                table: "Attributes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Users_RegisteredById",
                table: "Attributes",
                column: "RegisteredById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
