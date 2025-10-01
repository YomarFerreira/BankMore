using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string NumeroContaOuCpf { get; set; }
        public string Senha { get; set; }
    }

    public class LoginResponse
    {
        public bool Sucesso { get; set; }
        public string Token { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}