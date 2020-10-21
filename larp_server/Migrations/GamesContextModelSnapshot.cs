﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Models;

namespace larp_server.Migrations
{
    [DbContext(typeof(GamesContext))]
    partial class GamesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("larp_server.Models.Coord", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("RoomId")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("ConnectionID")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<bool>("IsConnected")
                        .HasColumnType("bit");

                    b.Property<double>("Latitude")
                        .HasColumnType("double");

                    b.Property<double>("Longitude")
                        .HasColumnType("double");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "RoomId");

                    b.HasIndex("RoomId");

                    b.ToTable("Coords");
                });

            modelBuilder.Entity("larp_server.Models.Player", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Email")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(80)")
                        .HasMaxLength(80);

                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("Password")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.HasKey("Token");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("larp_server.Models.Room", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("AdminName")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("AdminToken")
                        .HasColumnType("varchar(150)");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("datetime");

                    b.Property<string>("Password")
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.HasKey("Name");

                    b.HasIndex("AdminToken");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("larp_server.Models.Coord", b =>
                {
                    b.HasOne("larp_server.Models.Player", "Player")
                        .WithMany("CoordsList")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("larp_server.Models.Room", "Room")
                        .WithMany("CoordsList")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("larp_server.Models.Room", b =>
                {
                    b.HasOne("larp_server.Models.Player", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminToken");
                });
#pragma warning restore 612, 618
        }
    }
}
