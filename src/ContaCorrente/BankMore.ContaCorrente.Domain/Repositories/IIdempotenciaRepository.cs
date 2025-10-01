using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<Entities.Idempotencia> ObterPorChaveAsync(string chave);
        Task AdicionarAsync(Entities.Idempotencia idempotencia);
    }
}