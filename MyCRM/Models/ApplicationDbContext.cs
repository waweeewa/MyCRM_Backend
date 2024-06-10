using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyCRM.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Billing> Billing { get; set; }
        public virtual DbSet<Tariffs> Tariffs { get; set; }
        public virtual DbSet<UserDevices> UserDevices { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<VerifiedDevices> VerifiedDevices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-HMR91CS\\SQLEXPRESS;Database=MyCRM;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Billing>(entity =>
            {
                entity.HasKey(e => e.BId)
                    .HasName("PK__billing__DE9988FF826D9811");

                entity.ToTable("billing");

                entity.Property(e => e.BId).HasColumnName("bId");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.Paid).HasColumnName("paid");

                entity.Property(e => e.UsedPower).HasColumnName("usedPower");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Billing)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__billing__userId__4CA06362");
            });

            modelBuilder.Entity<Tariffs>(entity =>
            {
                entity.HasKey(e => e.TId)
                    .HasName("PK__tariffs__DC11576780189922");

                entity.ToTable("tariffs");

                entity.Property(e => e.TId).HasColumnName("tId");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("text");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<UserDevices>(entity =>
            {
                entity.HasKey(e => e.UdId)
                    .HasName("PK__userDevi__B8C47578B1B81BA8");

                entity.ToTable("userDevices");

                entity.Property(e => e.UdId).HasColumnName("udId");

                entity.Property(e => e.DeviceId).HasColumnName("deviceId");

                entity.Property(e => e.FromDate)
                    .HasColumnName("from_date")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("text");

                entity.Property(e => e.ToDate)
                    .HasColumnName("to_date")
                    .HasColumnType("date");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.UserDevices)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK__userDevic__devic__3E52440B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDevices)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__userDevic__userI__3D5E1FD2");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PK__users__DD771E5C8738946D");

                entity.ToTable("users");

                entity.Property(e => e.UId).HasColumnName("uId");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("text");

                entity.Property(e => e.Admincheck).HasColumnName("admincheck");

                entity.Property(e => e.Birthdate)
                    .HasColumnName("birthdate")
                    .HasColumnType("text");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("text");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasColumnType("text");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("text");

                entity.Property(e => e.FirstName)
                    .HasColumnName("firstName")
                    .HasColumnType("text");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasColumnType("text");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("text");

                entity.Property(e => e.TariffId).HasColumnName("tariffId");

                entity.Property(e => e.Zipcode).HasColumnName("zipcode");

                entity.HasOne(d => d.Tariff)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.TariffId)
                    .HasConstraintName("FK__users__tariffId__38996AB5");
            });

            modelBuilder.Entity<VerifiedDevices>(entity =>
            {
                entity.HasKey(e => e.VdId)
                    .HasName("PK__verified__70279D8F589E7834");

                entity.ToTable("verifiedDevices");

                entity.Property(e => e.VdId).HasColumnName("vdId");

                entity.Property(e => e.Model)
                    .HasColumnName("model")
                    .HasColumnType("text");

                entity.Property(e => e.SerialNum)
                    .HasColumnName("serialNum")
                    .HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
