using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBookingWeb.Migrations
{
    /// <inheritdoc />
    public partial class updateNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBEventEventCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories");

            migrationBuilder.RenameTable(
                name: "EventCategories",
                newName: "CategoryEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryEvents",
                table: "CategoryEvents",
                column: "CategoryEventId");

            migrationBuilder.CreateTable(
                name: "DBEventCategoryEvent",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CategoryEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBEventCategoryEvent", x => new { x.EventId, x.CategoryEventId });
                    table.ForeignKey(
                        name: "FK_DBEventCategoryEvent_CategoryEvents_CategoryEventId",
                        column: x => x.CategoryEventId,
                        principalTable: "CategoryEvents",
                        principalColumn: "CategoryEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DBEventCategoryEvent_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DBEventCategoryEvent_CategoryEventId",
                table: "DBEventCategoryEvent",
                column: "CategoryEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBEventCategoryEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryEvents",
                table: "CategoryEvents");

            migrationBuilder.RenameTable(
                name: "CategoryEvents",
                newName: "EventCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories",
                column: "CategoryEventId");

            migrationBuilder.CreateTable(
                name: "DBEventEventCategory",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBEventEventCategory", x => new { x.EventId, x.EventCategoryId });
                    table.ForeignKey(
                        name: "FK_DBEventEventCategory_EventCategories_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategories",
                        principalColumn: "CategoryEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DBEventEventCategory_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DBEventEventCategory_EventCategoryId",
                table: "DBEventEventCategory",
                column: "EventCategoryId");
        }
    }
}
