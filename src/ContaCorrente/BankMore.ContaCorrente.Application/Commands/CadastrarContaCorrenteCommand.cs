using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;

namespace BankMore.ContaCorrente.Application.Commands
{
    public class CadastrarContaCorrenteCommand : IRequest<CadastrarContaCorrenteResponse>
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
    }

    public class CadastrarContaCorrenteResponse
    {
        public bool Sucesso { get; set; }
        public int? NumeroConta { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}