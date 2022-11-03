using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClientTripExercise.Models
{
    public partial class s20192Context : DbContext
    {
        public s20192Context()
        {
        }

        public s20192Context(DbContextOptions<s20192Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<ClientTrip> ClientTrips { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Trip> Trips { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=db-mssql16.pjwstk.edu.pl;Initial Catalog=s20192;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.IdClient)
                    .HasName("Client_pk");

                entity.ToTable("Client");

                entity.Property(e => e.Email).HasMaxLength(120);

                entity.Property(e => e.FirstName).HasMaxLength(120);

                entity.Property(e => e.LastName).HasMaxLength(120);

                entity.Property(e => e.Pesel).HasMaxLength(120);

                entity.Property(e => e.Telephone).HasMaxLength(120);
            });

            modelBuilder.Entity<ClientTrip>(entity =>
            {
                entity.HasKey(e => new { e.IdClient, e.IdTrip })
                    .HasName("Client_Trip_pk");

                entity.ToTable("Client_Trip");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.RegisteredAt).HasColumnType("datetime");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Table_5_Client");

                entity.HasOne(d => d.IdTripNavigation)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdTrip)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Table_5_Trip");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.IdCountry)
                    .HasName("Country_pk");

                entity.ToTable("Country");

                entity.Property(e => e.Name).HasMaxLength(120);

                entity.HasMany(d => d.IdTrips)
                    .WithMany(p => p.IdCountries)
                    .UsingEntity<Dictionary<string, object>>(
                        "CountryTrip",
                        l => l.HasOne<Trip>().WithMany().HasForeignKey("IdTrip").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Country_Trip_Trip"),
                        r => r.HasOne<Country>().WithMany().HasForeignKey("IdCountry").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Country_Trip_Country"),
                        j =>
                        {
                            j.HasKey("IdCountry", "IdTrip").HasName("Country_Trip_pk");

                            j.ToTable("Country_Trip");
                        });
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.IdTrip)
                    .HasName("Trip_pk");

                entity.ToTable("Trip");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(220);

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
