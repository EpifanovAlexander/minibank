﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minibank.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Minibank.Data.Migrations
{
    [DbContext(typeof(MinibankContext))]
    [Migration("20220327115538_InitialDb")]
    partial class InitialDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Minibank.Data.DbModels.BankAccounts.BankAccountDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("Currency")
                        .HasColumnType("integer")
                        .HasColumnName("currency");

                    b.Property<DateTime?>("DateClosing")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_closing");

                    b.Property<DateTime>("DateOpening")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_opening");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<double>("Sum")
                        .HasColumnType("double precision")
                        .HasColumnName("sum");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_bank_account_id");

                    b.HasIndex("UserId");

                    b.ToTable("bank_accounts");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.BankTransferHistories.BankTransferHistoryDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("FromAccountId")
                        .HasColumnType("integer")
                        .HasColumnName("from_id");

                    b.Property<double>("Sum")
                        .HasColumnType("double precision")
                        .HasColumnName("sum");

                    b.Property<int>("ToAccountId")
                        .HasColumnType("integer")
                        .HasColumnName("to_id");

                    b.HasKey("Id")
                        .HasName("pk_transfer_history_id");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("transfer_history");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.Users.UserDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("Id")
                        .HasName("pk_user_id");

                    b.ToTable("user_table");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.BankAccounts.BankAccountDbModel", b =>
                {
                    b.HasOne("Minibank.Data.DbModels.Users.UserDbModel", "User")
                        .WithMany("BankAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.BankTransferHistories.BankTransferHistoryDbModel", b =>
                {
                    b.HasOne("Minibank.Data.DbModels.BankAccounts.BankAccountDbModel", "FromAccount")
                        .WithMany("FromTransferHistories")
                        .HasForeignKey("FromAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minibank.Data.DbModels.BankAccounts.BankAccountDbModel", "ToAccount")
                        .WithMany("ToTransferHistories")
                        .HasForeignKey("ToAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromAccount");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.BankAccounts.BankAccountDbModel", b =>
                {
                    b.Navigation("FromTransferHistories");

                    b.Navigation("ToTransferHistories");
                });

            modelBuilder.Entity("Minibank.Data.DbModels.Users.UserDbModel", b =>
                {
                    b.Navigation("BankAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}
