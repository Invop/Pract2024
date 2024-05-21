﻿// <auto-generated />
using System;
using ManekiApp.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    [DbContext(typeof(ManekiAppDBContext))]
    [Migration("20240521183452_ExtendedEntitiesAttribs")]
    partial class ExtendedEntitiesAttribs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ApplicationRoleApplicationUser", b =>
                {
                    b.Property<string>("RolesId")
                        .HasColumnType("text");

                    b.Property<string>("UsersId")
                        .HasColumnType("text");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ApplicationRoleApplicationUser", t =>
                        {
                            t.HasTrigger("ApplicationRoleApplicationUser_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ApplicationRole", t =>
                        {
                            t.HasTrigger("ApplicationRole_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("About")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("ProfilePicture")
                        .HasColumnType("bytea");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TelegramConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("TelegramId")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUser", t =>
                        {
                            t.HasTrigger("ApplicationUser_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("bytea");

                    b.Property<string>("SocialLinks")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("AuthorPage", "public", t =>
                        {
                            t.HasTrigger("AuthorPage_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Image", "public", t =>
                        {
                            t.HasTrigger("Image_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorPageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorPageId");

                    b.ToTable("Post", "public", t =>
                        {
                            t.HasTrigger("Post_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorPageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PermissionLevel")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorPageId");

                    b.ToTable("Subscription", "public", t =>
                        {
                            t.HasTrigger("Subscription_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.UserSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("boolean");

                    b.Property<bool>("ReceiveNotifications")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("SubscribedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("SubscriptionId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSubscription", "public", t =>
                        {
                            t.HasTrigger("UserSubscription_Trigger");
                        });
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserVerificationCode", "public", t =>
                        {
                            t.HasTrigger("UserVerificationCode_Trigger");
                        });
                });

            modelBuilder.Entity("ApplicationRoleApplicationUser", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ManekiApp.Server.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ApplicationUser", "User")
                        .WithOne("AuthorPage")
                        .HasForeignKey("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Image", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ManekiAppDB.Post", "Post")
                        .WithMany("Images")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Post", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", "AuthorPage")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthorPage");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Subscription", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", "AuthorPage")
                        .WithMany("Subscriptions")
                        .HasForeignKey("AuthorPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthorPage");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.UserSubscription", b =>
                {
                    b.HasOne("ManekiApp.Server.Models.ManekiAppDB.Subscription", "Subscription")
                        .WithMany("UserSubscriptions")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ManekiApp.Server.Models.ApplicationUser", "User")
                        .WithMany("UserSubscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subscription");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ApplicationUser", b =>
                {
                    b.Navigation("AuthorPage");

                    b.Navigation("UserSubscriptions");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.AuthorPage", b =>
                {
                    b.Navigation("Posts");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Post", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("ManekiApp.Server.Models.ManekiAppDB.Subscription", b =>
                {
                    b.Navigation("UserSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
