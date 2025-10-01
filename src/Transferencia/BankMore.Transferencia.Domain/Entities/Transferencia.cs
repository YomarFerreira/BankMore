using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.Entities;


namespace BankMore.Transferencia.Domain.Entities
{
    public class Transferencia : BaseEntity
    {
        public string IdContaCorrenteOrigem { get; private set; } = string.Empty;
        public string IdContaCorrenteDestino { get; private set; } = string.Empty;
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        public Transferencia(string idContaCorrenteOrigem, string idContaCorrenteDestino, decimal valor)
        {
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            Valor = valor;
            DataMovimento = DateTime.Now;
        }

        private Transferencia() { }
    }
}