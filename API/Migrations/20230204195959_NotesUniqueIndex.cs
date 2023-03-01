// using Microsoft.EntityFrameworkCore.Migrations;
//
// #nullable disable
//
// namespace Courses.API.Migrations
// {
//     /// <inheritdoc />
//     public partial class NotesUniqueIndex : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropIndex(
//                 name: "IX_Notes_EntryId",
//                 table: "Notes");
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_Notes_EntryId_CourseId",
//                 table: "Notes",
//                 columns: new[] { "EntryId", "CourseId" },
//                 unique: true);
//         }
//
//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropIndex(
//                 name: "IX_Notes_EntryId_CourseId",
//                 table: "Notes");
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_Notes_EntryId",
//                 table: "Notes",
//                 column: "EntryId");
//         }
//     }
// }
