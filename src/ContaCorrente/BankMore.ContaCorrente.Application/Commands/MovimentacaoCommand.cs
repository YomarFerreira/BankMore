using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class MovimentacaoCommand : IRequest<MovimentacaoResponse>
    {
        public string ChaveIdempotencia { get; set; }
        public string ContaCorrenteId { get; set; }
        public int? NumeroContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }

    public class MovimentacaoResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}