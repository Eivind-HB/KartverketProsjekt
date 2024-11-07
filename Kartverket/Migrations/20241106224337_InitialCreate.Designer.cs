﻿// <auto-generated />
using System;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kartverket.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241106224337_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Kartverket.Data.Case", b =>
                {
                    b.Property<int>("CaseNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CaseNo"));

                    b.Property<int>("CaseWorker_CaseWorkerID")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("Date")
                        .IsRequired()
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("Issue_IssueNr")
                        .HasColumnType("int");

                    b.Property<string>("LocationInfo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("User_UserID")
                        .HasColumnType("int");

                    b.HasKey("CaseNo");

                    b.ToTable("Case");
                });

            modelBuilder.Entity("Kartverket.Data.CaseWorker", b =>
                {
                    b.Property<int>("CaseWorkerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CaseWorkerID"));

                    b.Property<int>("CaseWorkerList_Case_CaseNo")
                        .HasColumnType("int");

                    b.Property<int>("KartverketEmployee_EmployeeID")
                        .HasColumnType("int");

                    b.HasKey("CaseWorkerID");

                    b.ToTable("CaseWorkers");
                });

            modelBuilder.Entity("Kartverket.Data.CaseWorkerList", b =>
                {
                    b.Property<int>("Case_CaseNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Case_CaseNo"));

                    b.Property<string>("AmountWorkers")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("CaseWorkerOverview_CaseWorkerID")
                        .HasColumnType("int");

                    b.HasKey("Case_CaseNo");

                    b.ToTable("CaseWorkerLists");
                });

            modelBuilder.Entity("Kartverket.Data.CaseWorkerOverview", b =>
                {
                    b.Property<int>("CaseWorkerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CaseWorkerId"));

                    b.Property<int>("CaseWorkerList_Case_CaseNo")
                        .HasColumnType("int");

                    b.Property<string>("PaidHours")
                        .HasColumnType("longtext");

                    b.HasKey("CaseWorkerId");

                    b.ToTable("CaseWorkerOverviews");
                });

            modelBuilder.Entity("Kartverket.Data.Issue", b =>
                {
                    b.Property<int>("issueNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("issueNo"));

                    b.Property<string>("IssueType")
                        .HasColumnType("longtext");

                    b.HasKey("issueNo");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("Kartverket.Data.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Mail")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
