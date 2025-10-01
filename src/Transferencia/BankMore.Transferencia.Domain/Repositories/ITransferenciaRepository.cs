using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMore.Transferencia.Domain.Repositories
{
    public interface ITransferenciaRepository
    {
        Task AdicionarAsync(Entities.Transferencia transferencia);
        Task<Entities.Transferencia> ObterPorIdAsync(string id);
        Task<List<Entities.Transferencia>> ObterPorContaCorrenteAsync(string idContaCorrente);
    }
}