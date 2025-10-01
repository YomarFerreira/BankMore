using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class InativarContaCorrenteCommand : IRequest<InativarContaCorrenteResponse>
    {
        public string ContaCorrenteId { get; set; }
        public string Senha { get; set; }
    }

    public class InativarContaCorrenteResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}