﻿// <auto-generated />
using System;
using Jwt.Identity.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Jwt.Identity.Data.Migrations
{
    [DbContext(typeof(IdentityContext))]
    [Migration("20230102172216_F2")]
    partial class F2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Jwt.Identity.Domain.Clients.Entity.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BaseUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EmailConfirmPage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailResetPage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lockout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LoginType")
                        .HasColumnType("int");

                    b.Property<string>("LoginUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignInExternal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignOut")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClientId");

                    b.HasIndex("ClientName")
                        .IsUnique()
                        .HasFilter("[ClientName] IS NOT NULL");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.IdentityPolicy.Entity.IdentitySettingPolicy", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CaptchStrategy")
                        .HasColumnType("int");

                    b.Property<int>("DefaultLockoutTimeSpanMinute")
                        .HasColumnType("int");

                    b.Property<int>("MaxFailedAccessAttempts")
                        .HasColumnType("int");

                    b.Property<bool>("RequireConfirmedAccount")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireDigit")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireLowercase")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireNonAlphanumeric")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireUppercase")
                        .HasColumnType("bit");

                    b.Property<int>("RequiredLength")
                        .HasColumnType("int");

                    b.Property<int>("RequiredUniqueChars")
                        .HasColumnType("int");

                    b.Property<int>("TokenLifespanHour")
                        .HasColumnType("int");

                    b.Property<int>("TotpLifeSpanMinute")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("IdentitySettings");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.Sessions.Entity.SessionEntity", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BrowserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeviceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IpAddress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SessionId");

                    b.ToTable("SessionEntity");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.UseLoginPolicy.Entities.UserLoginPolicyOptions", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApplicationUserPolicyPolicyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApplicationUserPolicyUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("NumberOfLogin")
                        .HasColumnType("int");

                    b.Property<int>("OvereNumberOfLogin")
                        .HasColumnType("int");

                    b.Property<string>("PolicyName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserPolicyUserId", "ApplicationUserPolicyPolicyId");

                    b.ToTable("UserLoginPolicyOptions");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Approved")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

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

                    b.Property<string>("UserLoginPolicyOptionId")
                        .HasColumnType("nvarchar(450)");

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

                    b.HasIndex("UserLoginPolicyOptionId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.ApplicationUserPolicy", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PolicyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApliApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "PolicyId");

                    b.HasIndex("ApliApplicationUserId");

                    b.ToTable("ApplicationUserPolicies");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.UserLogInOutLog", b =>
                {
                    b.Property<Guid>("IdGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SignInOut")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdGuid");

                    b.HasIndex("SessionId");

                    b.ToTable("UserLogInOutLogs");
                });

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

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.UseLoginPolicy.Entities.UserLoginPolicyOptions", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUserPolicy", null)
                        .WithMany()
                        .HasForeignKey("Id")
                        .HasPrincipalKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUserPolicy", null)
                        .WithMany("UserLoginPolicyOptions")
                        .HasForeignKey("ApplicationUserPolicyUserId", "ApplicationUserPolicyPolicyId");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.ApplicationUser", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.UseLoginPolicy.Entities.UserLoginPolicyOptions", "UserLoginPolicyOption")
                        .WithMany("ApplicationUsers")
                        .HasForeignKey("UserLoginPolicyOptionId");

                    b.Navigation("UserLoginPolicyOption");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.ApplicationUserPolicy", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", "ApliApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApliApplicationUserId");

                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApliApplicationUser");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.UserLogInOutLog", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.Sessions.Entity.SessionEntity", "Session")
                        .WithMany()
                        .HasForeignKey("SessionId");

                    b.Navigation("Session");
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
                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", null)
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

                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Jwt.Identity.Domain.User.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jwt.Identity.Domain.UseLoginPolicy.Entities.UserLoginPolicyOptions", b =>
                {
                    b.Navigation("ApplicationUsers");
                });

            modelBuilder.Entity("Jwt.Identity.Domain.User.Entities.ApplicationUserPolicy", b =>
                {
                    b.Navigation("UserLoginPolicyOptions");
                });
#pragma warning restore 612, 618
        }
    }
}
