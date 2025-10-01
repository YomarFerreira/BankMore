using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.Entities;


namespace BankMore.Tarifa.Domain.Entities
{
    public class Tarifa : BaseEntity
    {
        public string IdContaCorrente { get; private set; } = string.Empty;
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        public Tarifa(string idContaCorrente, decimal valor)
        {
            IdContaCorrente = idContaCorrente;
            Valor = valor;
            DataMovimento = DateTime.Now;
        }

        private Tarifa() { } // EF Constructor
    }
}