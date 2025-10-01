using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Entities;


namespace BankMore.ContaCorrente.Infrastructure.Data
{
    public class ContaCorrenteDbContext : DbContext
    {
        public ContaCorrenteDbContext(DbContextOptions<ContaCorrenteDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.ContaCorrente> ContasCorrentes { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ContaCorrente
            modelBuilder.Entity<Domain.Entities.ContaCorrente>(entity =>
            {
                entity.ToTable("contacorrente");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idcontacorrente").HasMaxLength(37);
                entity.Property(e => e.Numero).HasColumnName("numero");
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100);
                entity.Property(e => e.Ativo).HasColumnName("ativo").HasConversion<int>();
                entity.Property(e => e.Senha).HasColumnName("senha").HasMaxLength(100);
                entity.Property(e => e.Salt).HasColumnName("salt").HasMaxLength(100);
                entity.Ignore(e => e.Cpf);
                entity.Ignore(e => e.Movimentos);
                entity.HasIndex(e => e.Numero).IsUnique();
            });

            // Movimento
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.ToTable("movimento");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idmovimento").HasMaxLength(37);
                entity.Property(e => e.IdContaCorrente).HasColumnName("idcontacorrente").HasMaxLength(37);
                entity.Property(e => e.DataMovimento).HasColumnName("datamovimento")
                    .HasConversion(
                        v => v.ToString("dd/MM/yyyy"),
                        v => DateTime.ParseExact(v, "dd/MM/yyyy", null));
                entity.Property(e => e.TipoMovimento).HasColumnName("tipomovimento")
                    .HasConversion<string>();
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