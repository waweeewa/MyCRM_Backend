using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyCRM.DAL.DataModel
{
    public partial class PIS_DbContext : DbContext
    {
        public PIS_DbContext()
        {
        }

        public PIS_DbContext(DbContextOptions<PIS_DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PisUsersDResetar> PisUsersDResetar { get; set; }
        public virtual DbSet<Billing> Billings { get; set; }
        public virtual DbSet<VerifiedDevice> VerifiedDevices { get; set; }
        public virtual DbSet<Tariff> Tariffs { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<BillingArchive> BillingArchive { get; set; }
        public virtual DbSet<TariffArchive> TariffArchives { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Premještanje connection stringa iz koda.
                // optionsBuilder.UseSqlServer("Server=DESKTOP-HMR91CS\SQLEXPRESS;Database=MyCRM;);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("userDevices");

                entity.HasKey(e => e.udId);

                entity.Property(e => e.udId)
                    .HasColumnName("udId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.userId).HasColumnName("userId");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.from_date).HasColumnName("from_date");
                entity.Property(e => e.to_date).HasColumnName("to_date");
                entity.Property(e => e.name).HasColumnName("name");

            });
            modelBuilder.Entity<Billing>(entity =>
            {
                entity.ToTable("billing");

                entity.HasKey(e => e.BId);

                entity.Property(e => e.BId)
                    .HasColumnName("bId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.Paid).HasColumnName("paid");
                entity.Property(e => e.Month).HasColumnName("month");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.UsedPower).HasColumnName("usedPower");
                entity.Property(e => e.DeviceId).HasColumnName("deviceId");
                entity.Property(e => e.TarriffId).HasColumnName("tarriffId");

            });
            modelBuilder.Entity<BillingArchive>(entity =>
            {
                entity.ToTable("billingArchive");

                entity.HasKey(e => e.billArchId);

                entity.Property(e => e.billArchId)
                    .HasColumnName("billArchId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.billId).HasColumnName("billId");
                entity.Property(e => e.logUserId).HasColumnName("logUserId");

            });
            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.ToTable("tariffs");

                entity.HasKey(e => e.tId);
                entity.Property(e => e.tId)
                    .HasColumnName("tId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.price).HasColumnName("price");
            });
            modelBuilder.Entity<VerifiedDevice>(entity =>
            {
                entity.ToTable("verifiedDevices");

                entity.HasKey(e => e.vdId);
                entity.Property(e => e.vdId)
                    .HasColumnName("vdId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.model).HasColumnName("model");
                entity.Property(e => e.serialNum).HasColumnName("serialNum");
            });
            modelBuilder.Entity<PisUsersDResetar>(entity =>
            {
                entity.HasKey(e => e.uId)
                  .HasName("PK__users__DD771E5C8738946D");

                entity.ToTable("users");

                entity.Property(e => e.uId).HasColumnName("uId");

                entity.Property(e => e.firstName)
                    .HasColumnName("firstName")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.lastName)
                    .HasColumnName("lastName")
                    .HasMaxLength(128)
                    .IsUnicode(false);
                
                entity.Property(e => e.admincheck).HasColumnName("admincheck");

                entity.Property(e => e.email)
                    .HasColumnName("email")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.password)
                    .HasColumnName("password")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.address)
                    .HasColumnName("address")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.city)
                    .HasColumnName("city")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.tariffId).HasColumnName("tariffId");

                entity.Property(e => e.zipcode)
                    .HasColumnName("zipcode")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.birthdate)
                    .HasColumnName("birthdate")
                    .HasMaxLength(128)
                    .IsUnicode(false);

            });
            modelBuilder.Entity<TariffArchive>(entity =>
            {
                entity.ToTable("tariffsArchive");

                entity.HasKey(e => e.tAId);
                entity.Property(e => e.tAId)
                    .HasColumnName("tAId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.tariffId).HasColumnName("tariffId");
                entity.Property(e => e.month).HasColumnName("month");
                entity.Property(e => e.year).HasColumnName("year");
                entity.Property(e => e.priceArch).HasColumnName("priceArch");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
