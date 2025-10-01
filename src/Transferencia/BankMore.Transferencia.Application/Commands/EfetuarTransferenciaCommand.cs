using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;


namespace BankMore.Transferencia.Application.Commands
{
    public class EfetuarTransferenciaCommand : IRequest<EfetuarTransferenciaResponse>
    {
        public string ChaveIdempotencia { get; set; }
        public string ContaCorrenteId { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string Token { get; set; }
    }

    public class EfetuarTransferenciaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}