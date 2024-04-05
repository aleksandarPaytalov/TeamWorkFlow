﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeamWorkFlow.Infrastructure.Data;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    [DbContext(typeof(TeamWorkFlowDbContext))]
    partial class TeamWorkFlowDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Machine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Machine identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CalibrationSchedule")
                        .HasColumnType("datetime2")
                        .HasComment("Machine calibration schedule");

                    b.Property<int>("Capacity")
                        .HasColumnType("int")
                        .HasComment("Machine capacity");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasComment("Machine picture");

                    b.Property<bool>("IsCalibrated")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("MaintenanceScheduleEndDate")
                        .HasColumnType("datetime2")
                        .HasComment("Machine maintenanceScheduleEndDate");

                    b.Property<DateTime?>("MaintenanceScheduleStartDate")
                        .HasColumnType("datetime2")
                        .HasComment("Machine maintenanceScheduleStartDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("Machine name");

                    b.Property<double>("TotalMachineLoad")
                        .HasColumnType("float")
                        .HasComment("Machine total load");

                    b.HasKey("Id");

                    b.ToTable("Machines", (string)null);

                    b.HasComment("Machine db model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CalibrationSchedule = new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Capacity = 20,
                            ImageUrl = "",
                            IsCalibrated = false,
                            Name = "Zeiss Contura",
                            TotalMachineLoad = 0.0
                        },
                        new
                        {
                            Id = 2,
                            CalibrationSchedule = new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Capacity = 20,
                            ImageUrl = "",
                            IsCalibrated = false,
                            Name = "Zeiss O-inspect",
                            TotalMachineLoad = 0.0
                        },
                        new
                        {
                            Id = 3,
                            CalibrationSchedule = new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Capacity = 20,
                            ImageUrl = "",
                            IsCalibrated = false,
                            Name = "Zeiss Metrotom",
                            TotalMachineLoad = 0.0
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Operator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Operator identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AvailabilityStatusId")
                        .HasColumnType("int")
                        .HasComment("Operator status identifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int")
                        .HasComment("Operator working capacity in hours per day/shift");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("First and Last name of the operator");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasComment("Showing if the current operator is still working in the company");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasComment("Operator phoneNumber");

                    b.HasKey("Id");

                    b.HasIndex("AvailabilityStatusId");

                    b.ToTable("Operators", (string)null);

                    b.HasComment("Operator DB model");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.OperatorAvailabilityStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Operator identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)")
                        .HasComment("Availability status name");

                    b.HasKey("Id");

                    b.ToTable("OperatorAvailabilityStatusEnumerable", (string)null);

                    b.HasComment("Operator availability status db model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "at work"
                        },
                        new
                        {
                            Id = 2,
                            Name = "in sick leave"
                        },
                        new
                        {
                            Id = 3,
                            Name = "on vacation"
                        },
                        new
                        {
                            Id = 4,
                            Name = "on training"
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Part", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Part identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasComment("Part name");

                    b.Property<string>("PartArticleNumber")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasComment("Part article number");

                    b.Property<string>("PartClientNumber")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasComment("Client article number for the current part");

                    b.Property<string>("PartModel")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("PartStatusId")
                        .HasColumnType("int")
                        .HasComment("PartStatus identifier");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("ToolNumber")
                        .HasColumnType("int")
                        .HasComment("Part tool number");

                    b.HasKey("Id");

                    b.HasIndex("PartStatusId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Parts", (string)null);

                    b.HasComment("Part Db model");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.PartStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("PartStatus identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)")
                        .HasComment("PartStatus name");

                    b.HasKey("Id");

                    b.ToTable("PartStatusEnumerable", (string)null);

                    b.HasComment("Part status Db model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "released"
                        },
                        new
                        {
                            Id = 2,
                            Name = "not released"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Conditional released"
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Priority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Priority identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)")
                        .HasComment("Priority name");

                    b.HasKey("Id");

                    b.ToTable("Priorities", (string)null);

                    b.HasComment("Priority data model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "low"
                        },
                        new
                        {
                            Id = 2,
                            Name = "normal"
                        },
                        new
                        {
                            Id = 3,
                            Name = "high"
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Project identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Appliance")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Project appliance sector");

                    b.Property<string>("ClientName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Client name");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Project name");

                    b.Property<string>("ProjectNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasComment("Project number");

                    b.Property<int>("ProjectStatusId")
                        .HasColumnType("int")
                        .HasComment("ProjectStatus identifier");

                    b.Property<int>("TotalHoursSpent")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectStatusId");

                    b.ToTable("Projects", (string)null);

                    b.HasComment("Project data model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Appliance = "Automotive industry",
                            ClientName = "Bmw",
                            ProjectName = "BMW Housing Gx9",
                            ProjectNumber = "249100",
                            ProjectStatusId = 1,
                            TotalHoursSpent = 50
                        },
                        new
                        {
                            Id = 2,
                            Appliance = "Automotive industry",
                            ClientName = "Vw",
                            ProjectName = "Vw Tuareg Front panel ",
                            ProjectNumber = "249200",
                            ProjectStatusId = 2,
                            TotalHoursSpent = 20
                        },
                        new
                        {
                            Id = 3,
                            Appliance = "Automotive industry",
                            ClientName = "Toyota",
                            ProjectName = "Toyota Climatic module X5",
                            ProjectNumber = "249300",
                            ProjectStatusId = 3,
                            TotalHoursSpent = 41
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.ProjectStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("ProjectStatus identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasComment("ProjectStatus name");

                    b.HasKey("Id");

                    b.ToTable("ProjectStatusEnumerable", (string)null);

                    b.HasComment("ProjectStatus data model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "In production"
                        },
                        new
                        {
                            Id = 2,
                            Name = "In development"
                        },
                        new
                        {
                            Id = 3,
                            Name = "in ACL"
                        });
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Task identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Attachment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasComment("Task attachments - files, drawings, documents, etc.");

                    b.Property<string>("Comment")
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)")
                        .HasComment("Comment for the current task");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasComment("Task creator identifier");

                    b.Property<DateTime?>("DeadLine")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .HasColumnType("nvarchar(1500)")
                        .HasComment("Task description");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2")
                        .HasComment("The date when the task is finished");

                    b.Property<int>("EstimatedTime")
                        .HasColumnType("int")
                        .HasComment("Estimated time for the Task that is needed to be complete - in hours");

                    b.Property<int?>("MachineId")
                        .HasColumnType("int")
                        .HasComment("Machine identifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Task Name");

                    b.Property<int>("PriorityId")
                        .HasColumnType("int")
                        .HasComment("Priority identifier");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2")
                        .HasComment("Task starting date");

                    b.Property<int>("TaskStatusId")
                        .HasColumnType("int")
                        .HasComment("TaskStatus identifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("MachineId");

                    b.HasIndex("PriorityId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TaskStatusId");

                    b.ToTable("Tasks", (string)null);

                    b.HasComment("Task Db model");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.TaskOperator", b =>
                {
                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("OperatorId", "TaskId");

                    b.HasIndex("TaskId");

                    b.ToTable("TasksOperators", (string)null);

                    b.HasComment("TaskOperator data model");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("TaskStatus identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasComment("TaskStatus name");

                    b.HasKey("Id");

                    b.ToTable("TaskStatusEnumerable", (string)null);

                    b.HasComment("TaskStatus data model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "open"
                        },
                        new
                        {
                            Id = 2,
                            Name = "in progress"
                        },
                        new
                        {
                            Id = 3,
                            Name = "finished"
                        },
                        new
                        {
                            Id = 4,
                            Name = "canceled"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Operator", b =>
                {
                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.OperatorAvailabilityStatus", "AvailabilityStatus")
                        .WithMany("Operators")
                        .HasForeignKey("AvailabilityStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AvailabilityStatus");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Part", b =>
                {
                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.PartStatus", "PartStatus")
                        .WithMany("Parts")
                        .HasForeignKey("PartStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Project", "Project")
                        .WithMany("Parts")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PartStatus");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Project", b =>
                {
                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.ProjectStatus", "ProjectStatus")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectStatus");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Task", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Machine", "Machine")
                        .WithMany("Tasks")
                        .HasForeignKey("MachineId");

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Priority", "Priority")
                        .WithMany("Tasks")
                        .HasForeignKey("PriorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.TaskStatus", "TaskStatus")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Machine");

                    b.Navigation("Priority");

                    b.Navigation("Project");

                    b.Navigation("TaskStatus");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.TaskOperator", b =>
                {
                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Operator", "Operator")
                        .WithMany("TasksOperators")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeamWorkFlow.Infrastructure.Data.Models.Task", "Task")
                        .WithMany("TasksOperators")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Operator");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Machine", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Operator", b =>
                {
                    b.Navigation("TasksOperators");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.OperatorAvailabilityStatus", b =>
                {
                    b.Navigation("Operators");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.PartStatus", b =>
                {
                    b.Navigation("Parts");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Priority", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Project", b =>
                {
                    b.Navigation("Parts");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.ProjectStatus", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.Task", b =>
                {
                    b.Navigation("TasksOperators");
                });

            modelBuilder.Entity("TeamWorkFlow.Infrastructure.Data.Models.TaskStatus", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
