using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.API.Migrations
{
    /// <inheritdoc />
    public partial class WatchedRelationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_Watched_CourseId",
            //     table: "Watched");

            migrationBuilder.CreateIndex(
                name: "IX_Watched_CourseId",
                table: "Watched",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Watched_CourseId",
                table: "Watched");

            migrationBuilder.CreateIndex(
                name: "IX_Watched_CourseId",
                table: "Watched",
                column: "CourseId",
                unique: true);
        }
    }
}
