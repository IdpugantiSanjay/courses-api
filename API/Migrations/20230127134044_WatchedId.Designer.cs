﻿// // <auto-generated />
// using System;
// using Courses.API.Database;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure;
// using Microsoft.EntityFrameworkCore.Migrations;
// using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
// using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
//
// #nullable disable
//
// namespace Courses.API.Migrations
// {
//     [DbContext(typeof(AppDbContext))]
//     [Migration("20230127134044_WatchedId")]
//     partial class WatchedId
//     {
//         protected override void BuildTargetModel(ModelBuilder modelBuilder)
//         {
// #pragma warning disable 612, 618
//             modelBuilder
//                 .HasAnnotation("ProductVersion", "6.0.8")
//                 .HasAnnotation("Relational:MaxIdentifierLength", 63);
//
//             NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
//
//             modelBuilder.Entity("API.Courses.Author", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("integer");
//
//                     NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
//
//                     b.Property<string>("Name")
//                         .IsRequired()
//                         .HasMaxLength(64)
//                         .HasColumnType("character varying(64)");
//
//                     b.HasKey("Id");
//
//                     b.ToTable("Authors");
//                 });
//
//             modelBuilder.Entity("API.Courses.Course", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("integer");
//
//                     NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
//
//                     b.Property<int?>("AuthorId")
//                         .HasColumnType("integer");
//
//                     b.Property<string[]>("Categories")
//                         .HasColumnType("text[]");
//
//                     b.Property<long>("Duration")
//                         .HasColumnType("bigint");
//
//                     b.Property<string>("Host")
//                         .IsRequired()
//                         .HasMaxLength(64)
//                         .HasColumnType("character varying(64)");
//
//                     b.Property<bool>("IsHighDefinition")
//                         .HasColumnType("boolean");
//
//                     b.Property<string>("Name")
//                         .IsRequired()
//                         .HasMaxLength(128)
//                         .HasColumnType("character varying(128)");
//
//                     b.Property<string>("Path")
//                         .IsRequired()
//                         .HasMaxLength(512)
//                         .HasColumnType("character varying(512)");
//
//                     b.Property<int?>("PlatformId")
//                         .HasColumnType("integer");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("AuthorId");
//
//                     b.HasIndex("Name")
//                         .IsUnique();
//
//                     b.HasIndex("PlatformId");
//
//                     b.ToTable("Courses");
//                 });
//
//             modelBuilder.Entity("API.Courses.CourseEntry", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("integer");
//
//                     NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
//
//                     b.Property<int>("CourseId")
//                         .HasColumnType("integer");
//
//                     b.Property<long>("Duration")
//                         .HasColumnType("bigint");
//
//                     b.Property<string>("Name")
//                         .IsRequired()
//                         .HasMaxLength(128)
//                         .HasColumnType("character varying(128)");
//
//                     b.Property<string>("Section")
//                         .HasMaxLength(128)
//                         .HasColumnType("character varying(128)");
//
//                     b.Property<int>("SequenceNumber")
//                         .HasColumnType("integer");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("CourseId");
//
//                     b.ToTable("CourseEntries");
//                 });
//
//             modelBuilder.Entity("API.Courses.Platform", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("integer");
//
//                     NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
//
//                     b.Property<string>("Name")
//                         .IsRequired()
//                         .HasMaxLength(64)
//                         .HasColumnType("character varying(64)");
//
//                     b.HasKey("Id");
//
//                     b.ToTable("Platforms");
//                 });
//
//             modelBuilder.Entity("API.Watched.Watched", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("integer");
//
//                     NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
//
//                     b.Property<int>("CourseId")
//                         .HasColumnType("integer");
//
//                     b.Property<DateTimeOffset>("CreatedAt")
//                         .HasColumnType("timestamp with time zone");
//
//                     b.Property<int>("EntryId")
//                         .HasColumnType("integer");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("EntryId");
//
//                     b.ToTable("Watched");
//                 });
//
//             modelBuilder.Entity("API.Courses.Course", b =>
//                 {
//                     b.HasOne("API.Courses.Author", "Author")
//                         .WithMany()
//                         .HasForeignKey("AuthorId");
//
//                     b.HasOne("API.Courses.Platform", "Platform")
//                         .WithMany()
//                         .HasForeignKey("PlatformId");
//
//                     b.Navigation("Author");
//
//                     b.Navigation("Platform");
//                 });
//
//             modelBuilder.Entity("API.Courses.CourseEntry", b =>
//                 {
//                     b.HasOne("API.Courses.Course", null)
//                         .WithMany("Entries")
//                         .HasForeignKey("CourseId")
//                         .OnDelete(DeleteBehavior.Cascade)
//                         .IsRequired();
//                 });
//
//             modelBuilder.Entity("API.Watched.Watched", b =>
//                 {
//                     b.HasOne("API.Courses.CourseEntry", "Entry")
//                         .WithMany()
//                         .HasForeignKey("EntryId")
//                         .OnDelete(DeleteBehavior.Cascade)
//                         .IsRequired();
//
//                     b.Navigation("Entry");
//                 });
//
//             modelBuilder.Entity("API.Courses.Course", b =>
//                 {
//                     b.Navigation("Entries");
//                 });
// #pragma warning restore 612, 618
//         }
//     }
// }
