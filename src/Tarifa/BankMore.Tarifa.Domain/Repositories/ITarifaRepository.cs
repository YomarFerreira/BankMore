using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankMore.Tarifa.Domain.Repositories
{
    public interface ITarifaRepository
    {
        Task AdicionarAsync(Entities.Tarifa tarifa);
        Task<List<Entities.Tarifa>> ObterPorContaCorrenteAsync(string idContaCorrente);
    }
}