using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShroomCity.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAttributeMushroomsrelationtoMMforrealthistime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_Mushrooms_MushroomId",
                table: "Attributes");

            migrationBuilder.DropIndex(
                name: "IX_Attributes_MushroomId",
                table: "Attributes");

            migrationBuilder.DropColumn(
                name: "MushroomId",
                table: "Attributes");

            migrationBuilder.CreateTable(
                name: "AttributeMushroom",
                columns: table => new
                {
                    AttributesId = table.Column<int>(type: "integer", nullable: false),
                    mushroomsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeMushroom", x => new { x.AttributesId, x.mushroomsId });
                    table.ForeignKey(
                        name: "FK_AttributeMushroom_Attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeMushroom_Mushrooms_mushroomsId",
                        column: x => x.mushroomsId,
                        principalTable: "Mushrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMushroom_mushroomsId",
                table: "AttributeMushroom",
                column: "mushroomsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttributeMushroom");

            migrationBuilder.AddColumn<int>(
                name: "MushroomId",
                table: "Attributes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_MushroomId",
                table: "Attributes",
                column: "MushroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Mushrooms_MushroomId",
                table: "Attributes",
                column: "MushroomId",
                principalTable: "Mushrooms",
                principalColumn: "Id");
        }
    }
}
