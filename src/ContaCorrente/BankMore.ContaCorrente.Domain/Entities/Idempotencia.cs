using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Entities
{
    public class Idempotencia
    {
        public string ChaveIdempotencia { get; private set; } = string.Empty;
        public string Requisicao { get; private set; } = string.Empty;
        public string Resultado { get; private set; } = string.Empty;

        public Idempotencia(string chaveIdempotencia, string requisicao, string resultado)
        {
            ChaveIdempotencia = chaveIdempotencia;
            Requisicao = requisicao;
            Resultado = resultado;
        }

        private Idempotencia() { }
    }
}