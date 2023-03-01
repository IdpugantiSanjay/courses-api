// using Microsoft.EntityFrameworkCore.Migrations;
//
// #nullable disable
//
// namespace Courses.API.Migrations
// {
//     /// <inheritdoc />
//     public partial class Playlist : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.AlterColumn<string>(
//                 name: "Path",
//                 table: "Courses",
//                 type: "character varying(512)",
//                 maxLength: 512,
//                 nullable: true,
//                 oldClrType: typeof(string),
//                 oldType: "character varying(512)",
//                 oldMaxLength: 512);
//
//             migrationBuilder.AlterColumn<string>(
//                 name: "Host",
//                 table: "Courses",
//                 type: "character varying(64)",
//                 maxLength: 64,
//                 nullable: true,
//                 oldClrType: typeof(string),
//                 oldType: "character varying(64)",
//                 oldMaxLength: 64);
//
//             migrationBuilder.AddColumn<string>(
//                 name: "PlaylistId",
//                 table: "Courses",
//                 type: "text",
//                 nullable: true);
//
//             migrationBuilder.AddColumn<string>(
//                 name: "VideoId",
//                 table: "CourseEntries",
//                 type: "text",
//                 nullable: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_Notes_EntryId",
//                 table: "Notes",
//                 column: "EntryId",
//                 unique: true);
//         }
//
//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropIndex(
//                 name: "IX_Notes_EntryId",
//                 table: "Notes");
//
//             migrationBuilder.DropColumn(
//                 name: "PlaylistId",
//                 table: "Courses");
//
//             migrationBuilder.DropColumn(
//                 name: "VideoId",
//                 table: "CourseEntries");
//
//             migrationBuilder.AlterColumn<string>(
//                 name: "Path",
//                 table: "Courses",
//                 type: "character varying(512)",
//                 maxLength: 512,
//                 nullable: false,
//                 defaultValue: "",
//                 oldClrType: typeof(string),
//                 oldType: "character varying(512)",
//                 oldMaxLength: 512,
//                 oldNullable: true);
//
//             migrationBuilder.AlterColumn<string>(
//                 name: "Host",
//                 table: "Courses",
//                 type: "character varying(64)",
//                 maxLength: 64,
//                 nullable: false,
//                 defaultValue: "",
//                 oldClrType: typeof(string),
//                 oldType: "character varying(64)",
//                 oldMaxLength: 64,
//                 oldNullable: true);
//         }
//     }
// }
