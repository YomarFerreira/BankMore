using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.Entities;
using BankMore.Shared.Domain.Enums;

namespace BankMore.ContaCorrente.Domain.Entities
{
    public class Movimento : BaseEntity
    {
        public string IdContaCorrente { get; private set; } = string.Empty;
        public DateTime DataMovimento { get; private set; }
        public TipoMovimento TipoMovimento { get; private set; }
        public decimal Valor { get; private set; }

        public Movimento(string idContaCorrente, TipoMovimento tipoMovimento, decimal valor)
        {
            IdContaCorrente = idContaCorrente;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            DataMovimento = DateTime.Now;
        }

        private Movimento() { }
    }
}