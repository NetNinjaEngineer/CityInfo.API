using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Population = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointOfInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointOfInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointOfInterests_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "Latitude", "Longitude", "Name", "Population" },
                values: new object[,]
                {
                    { 1, "USA", 40.712800000000001, -74.006, "New York", 8398748 },
                    { 2, "UK", 51.509900000000002, -0.11799999999999999, "London", 8982000 },
                    { 3, "Japan", 35.689500000000002, 139.6917, "Tokyo", 13929286 }
                });

            migrationBuilder.InsertData(
                table: "PointOfInterests",
                columns: new[] { "Id", "Category", "CityId", "Description", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "Park", 1, "A large urban park in Manhattan.", 40.7851, -73.968299999999999, "Central Park" },
                    { 2, "Landmark", 1, "Iconic skyscraper in Midtown Manhattan.", 40.748399999999997, -73.985699999999994, "Empire State Building" },
                    { 3, "Park", 2, "One of the largest parks in London.", 51.507399999999997, -0.16569999999999999, "Hyde Park" },
                    { 4, "Museum", 2, "World-famous museum of art and antiquities.", 51.519399999999997, -0.127, "British Museum" },
                    { 5, "Park", 3, "Famous public park in central Tokyo.", 35.714599999999997, 139.7732, "Ueno Park" },
                    { 6, "Landmark", 3, "Iconic communications and observation tower.", 35.6586, 139.74539999999999, "Tokyo Tower" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointOfInterests_CityId",
                table: "PointOfInterests",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointOfInterests");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
