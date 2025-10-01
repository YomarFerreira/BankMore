using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Shared.Domain.Enums;

namespace BankMore.ContaCorrente.Application.Queries
{
    public class ConsultarSaldoQuery : IRequest<ConsultarSaldoResponse>
    {
        public string ContaCorrenteId { get; set; }
    }

    public class ConsultarSaldoResponse
    {
        public bool Sucesso { get; set; }
        public int? NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public DateTime? DataConsulta { get; set; }
        public decimal? Saldo { get; set; }
        public string Mensagem { get; set; }
        public TipoFalha? TipoFalha { get; set; }
    }
}