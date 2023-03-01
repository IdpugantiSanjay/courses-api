using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseModule.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNamingConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course");

            migrationBuilder.CreateTable(
                name: "courses",
                schema: "course",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    duration = table.Column<long>(type: "bigint", nullable: false),
                    categories = table.Column<string[]>(type: "text[]", nullable: true),
                    ishighdefinition = table.Column<bool>(name: "is_high_definition", type: "boolean", nullable: false),
                    playlistid = table.Column<string>(name: "playlist_id", type: "text", nullable: true),
                    path = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    host = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "course_entry",
                schema: "course",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    courseid = table.Column<int>(name: "course_id", type: "integer", nullable: false),
                    videoid = table.Column<string>(name: "video_id", type: "text", nullable: true),
                    sequencenumber = table.Column<int>(name: "sequence_number", type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    duration = table.Column<long>(type: "bigint", nullable: false),
                    section = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_entry", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_entry_courses_course_id",
                        column: x => x.courseid,
                        principalSchema: "course",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_course_entry_course_id",
                schema: "course",
                table: "course_entry",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_name",
                schema: "course",
                table: "courses",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_entry",
                schema: "course");

            migrationBuilder.DropTable(
                name: "courses",
                schema: "course");
        }
    }
}
