using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankMore.Tarifa.Domain.Services
{
    public interface IContaCorrenteService
    {
        Task<bool> EfetuarDebitoTarifaAsync(string contaCorrenteId, decimal valor);
    }
}