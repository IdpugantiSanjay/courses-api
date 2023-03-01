// using Microsoft.EntityFrameworkCore.Migrations;
// using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
//
// #nullable disable
//
// namespace Courses.API.Migrations
// {
//     /// <inheritdoc />
//     public partial class Notes : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.CreateTable(
//                 name: "Notes",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "integer", nullable: false)
//                         .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
//                     CourseId = table.Column<int>(type: "integer", nullable: false),
//                     EntryId = table.Column<int>(type: "integer", nullable: false),
//                     Note = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_Notes", x => x.Id);
//                     table.ForeignKey(
//                         name: "FK_Notes_CourseEntries_EntryId",
//                         column: x => x.EntryId,
//                         principalTable: "CourseEntries",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Cascade);
//                     table.ForeignKey(
//                         name: "FK_Notes_Courses_CourseId",
//                         column: x => x.CourseId,
//                         principalTable: "Courses",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Cascade);
//                 });
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_Notes_CourseId",
//                 table: "Notes",
//                 column: "CourseId");
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_Notes_EntryId",
//                 table: "Notes",
//                 column: "EntryId");
//         }
//
//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropTable(
//                 name: "Notes");
//         }
//     }
// }
