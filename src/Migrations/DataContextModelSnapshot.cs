// <auto-generated />
using System;
using Locker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Locker.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Locker.Models.Entities.Account", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateLastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RoleID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TenantID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("RoleID");

                    b.HasIndex("TenantID");

                    b.HasIndex("UserID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Locker.Models.Entities.Role", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateLastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TenantID")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasAlternateKey("Name");

                    b.HasIndex("TenantID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Locker.Models.Entities.Tenant", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("text");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateLastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("Locker.Models.Entities.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateLastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("SecurityStamp")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Locker.Models.Entities.Account", b =>
                {
                    b.HasOne("Locker.Models.Entities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Locker.Models.Entities.Tenant", "Tenant")
                        .WithMany("Accounts")
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Locker.Models.Entities.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Tenant");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Locker.Models.Entities.Role", b =>
                {
                    b.HasOne("Locker.Models.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Locker.Models.Entities.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Locker.Models.Entities.Tenant", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Locker.Models.Entities.User", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
