using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.Tarifa.Domain.Entities;


namespace BankMore.Tarifa.Infrastructure.Data
{
    public class TarifaDbContext : DbContext
    {
        public TarifaDbContext(DbContextOptions<TarifaDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Tarifa> Tarifas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tarifa
            modelBuilder.Entity<Domain.Entities.Tarifa>(entity =>
            {
                entity.ToTable("tarifa");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idtarifa").HasMaxLength(37);
                entity.Property(e => e.IdContaCorrente).HasColumnName("idcontacorrente").HasMaxLength(37);
                entity.Property(e => e.DataMovimento).HasColumnName("datamovimento")
                    .HasConversion(
                        v => v.ToString("dd/MM/yyyy"),
                        v => DateTime.ParseExact(v, "dd/MM/yyyy", null));
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("REAL");
            });
        }
    }
}