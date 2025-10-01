using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Shared.Domain.ValueObjects;

namespace BankMore.ContaCorrente.Domain.Repositories
{
    public interface IContaCorrenteRepository
    {
        Task<Entities.ContaCorrente> ObterPorIdAsync(string id);
        Task<Entities.ContaCorrente> ObterPorNumeroAsync(int numero);
        Task<Entities.ContaCorrente> ObterPorCpfAsync(Cpf cpf);
        Task<bool> ExistePorCpfAsync(Cpf cpf);
        Task<bool> ExistePorNumeroAsync(int numero);
        Task AdicionarAsync(Entities.ContaCorrente contaCorrente);
        Task AtualizarAsync(Entities.ContaCorrente contaCorrente);
        Task<List<Entities.Movimento>> ObterMovimentosAsync(string idContaCorrente);
        Task AdicionarMovimentoAsync(Entities.Movimento movimento);
    }
}