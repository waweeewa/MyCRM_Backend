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


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
