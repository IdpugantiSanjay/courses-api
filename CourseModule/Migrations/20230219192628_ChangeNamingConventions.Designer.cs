﻿// <auto-generated />
using CourseModule.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseModule.Migrations
{
    [DbContext(typeof(CourseDbContext))]
    [Migration("20230219192628_ChangeNamingConventions")]
    partial class ChangeNamingConventions
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("course")
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CourseModule.Entities.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string[]>("Categories")
                        .HasColumnType("text[]")
                        .HasColumnName("categories");

                    b.Property<long>("Duration")
                        .HasColumnType("bigint")
                        .HasColumnName("duration");

                    b.Property<string>("Host")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("host");

                    b.Property<bool>("IsHighDefinition")
                        .HasColumnType("boolean")
                        .HasColumnName("is_high_definition");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<string>("Path")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("path");

                    b.Property<string>("PlaylistId")
                        .HasColumnType("text")
                        .HasColumnName("playlist_id");

                    b.HasKey("Id")
                        .HasName("pk_courses");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_courses_name");

                    b.ToTable("courses", "course");
                });

            modelBuilder.Entity("CourseModule.Entities.CourseEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("integer")
                        .HasColumnName("course_id");

                    b.Property<long>("Duration")
                        .HasColumnType("bigint")
                        .HasColumnName("duration");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<string>("Section")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("section");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("integer")
                        .HasColumnName("sequence_number");

                    b.Property<string>("VideoId")
                        .HasColumnType("text")
                        .HasColumnName("video_id");

                    b.HasKey("Id")
                        .HasName("pk_course_entry");

                    b.HasIndex("CourseId")
                        .HasDatabaseName("ix_course_entry_course_id");

                    b.ToTable("course_entry", "course");
                });

            modelBuilder.Entity("CourseModule.Entities.CourseEntry", b =>
                {
                    b.HasOne("CourseModule.Entities.Course", null)
                        .WithMany("Entries")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_course_entry_courses_course_id");
                });

            modelBuilder.Entity("CourseModule.Entities.Course", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
