// using Microsoft.EntityFrameworkCore.Migrations;
//
// #nullable disable
//
// namespace Courses.API.Migrations
// {
//     /// <inheritdoc />
//     public partial class WatchedRelation : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             // migrationBuilder.CreateIndex(
//             //     name: "IX_Watched_CourseId",
//             //     table: "Watched",
//             //     column: "CourseId",
//             //     unique: true);
//
//             migrationBuilder.AddForeignKey(
//                 name: "FK_Watched_Courses_CourseId",
//                 table: "Watched",
//                 column: "CourseId",
//                 principalTable: "Courses",
//                 principalColumn: "Id",
//                 onDelete: ReferentialAction.Cascade);
//         }
//
//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropForeignKey(
//                 name: "FK_Watched_Courses_CourseId",
//                 table: "Watched");
//
//             // migrationBuilder.DropIndex(
//             //     name: "IX_Watched_CourseId",
//             //     table: "Watched");
//         }
//     }
// }
