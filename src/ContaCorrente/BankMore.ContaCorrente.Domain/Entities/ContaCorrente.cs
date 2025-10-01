using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.Entities;
using BankMore.Shared.Domain.ValueObjects;

namespace BankMore.ContaCorrente.Domain.Entities
{
    public class ContaCorrente : BaseEntity
    {
        public int Numero { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public bool Ativo { get; private set; }
        public string Senha { get; private set; } = string.Empty;
        public string Salt { get; private set; } = string.Empty;
        public Cpf Cpf { get; private set; } = null!;

        private readonly List<Movimento> _movimentos = new();
        public IReadOnlyCollection<Movimento> Movimentos => _movimentos.AsReadOnly();

        public ContaCorrente(string nome, Cpf cpf, string senhaHash, string salt)
        {
            Nome = nome;
            Cpf = cpf;
            Senha = senhaHash;
            Salt = salt;
            Ativo = true;
            Numero = GerarNumeroContaCorrente();
        }

        private ContaCorrente() { } // EF Constructor

        public void Inativar()
        {
            Ativo = false;
        }

        public void AdicionarMovimento(Movimento movimento)
        {
            _movimentos.Add(movimento);
        }

        public decimal CalcularSaldo()
        {
            var creditos = _movimentos
                .Where(m => m.TipoMovimento == Shared.Domain.Enums.TipoMovimento.Credito)
                .Sum(m => m.Valor);

            var debitos = _movimentos
                .Where(m => m.TipoMovimento == Shared.Domain.Enums.TipoMovimento.Debito)
                .Sum(m => m.Valor);

            return creditos - debitos;
        }

        private int GerarNumeroContaCorrente()
        {
            return new Random().Next(100000000, 999999999);
        }
    }
}