﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecurityQuestionAuthAPI;

#nullable disable

namespace SecurityQuestionAuthAPI.Migrations
{
    [DbContext(typeof(SecurityAuthContext))]
    partial class SecurityAuthContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.OTP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TIMESTAMP")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP()");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Otp");
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.Phrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("IV")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PhraseKey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PhraseText")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserEmail")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("phrases");
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.SecurityQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("Answer")
                        .IsRequired()
                        .HasColumnType("varbinary(3072)");

                    b.Property<byte[]>("AnswerKey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Answer")
                        .IsUnique();

                    b.HasIndex("QuestionText")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("securityQuestions", (string)null);
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PasswordKey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.OTP", b =>
                {
                    b.HasOne("SecurityQuestionAuthAPI.Data.User", "user")
                        .WithMany("otp")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.Phrase", b =>
                {
                    b.HasOne("SecurityQuestionAuthAPI.Data.User", "user")
                        .WithMany("phrase")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.SecurityQuestion", b =>
                {
                    b.HasOne("SecurityQuestionAuthAPI.Data.User", "User")
                        .WithMany("securityQuestions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SecurityQuestionAuthAPI.Data.User", b =>
                {
                    b.Navigation("otp");

                    b.Navigation("phrase");

                    b.Navigation("securityQuestions");
                });
#pragma warning restore 612, 618
        }
    }
}
