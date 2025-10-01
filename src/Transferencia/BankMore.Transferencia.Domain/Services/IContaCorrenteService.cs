using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMore.Transferencia.Domain.Services
{
    public interface IContaCorrenteService
    {
        Task<bool> ValidarContaCorrenteAsync(string contaCorrenteId, string token);
        Task<bool> EfetuarMovimentacaoAsync(string chaveIdempotencia, int? numeroContaCorrente, decimal valor, char tipoMovimento, string token);
    }
}