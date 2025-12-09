using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class addStationGeoIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS idx_stations_geo_location ON core.stations USING gist (ll_to_earth(latitude, longitude));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS core.idx_stations_geo_location;");
        }
    }
}
