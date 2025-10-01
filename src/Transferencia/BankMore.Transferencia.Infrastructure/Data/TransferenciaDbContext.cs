using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.Transferencia.Domain.Entities;


namespace BankMore.Transferencia.Infrastructure.Data
{
    public class TransferenciaDbContext : DbContext
    {
        public TransferenciaDbContext(DbContextOptions<TransferenciaDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Transferencia> Transferencias { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Transferencia
            modelBuilder.Entity<Domain.Entities.Transferencia>(entity =>
            {
                entity.ToTable("transferencia");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idtransferencia").HasMaxLength(37);
                entity.Property(e => e.IdContaCorrenteOrigem).HasColumnName("idcontacorrente_origem").HasMaxLength(37);
                entity.Property(e => e.IdContaCorrenteDestino).HasColumnName("idcontacorrente_destino").HasMaxLength(37);
                entity.Property(e => e.DataMovimento).HasColumnName("datamovimento")
                    .HasConversion(
                        v => v.ToString("dd/MM/yyyy"),
                        v => DateTime.ParseExact(v, "dd/MM/yyyy", null));
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("REAL");
            });

            // Idempotencia
            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.ToTable("idempotencia");
                entity.HasKey(e => e.ChaveIdempotencia);
                entity.Property(e => e.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(37);
                entity.Property(e => e.Requisicao).HasColumnName("requisicao").HasMaxLength(1000);
                entity.Property(e => e.Resultado).HasColumnName("resultado").HasMaxLength(1000);
            });
        }
    }
}