using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.Shared.Domain.ValueObjects;
using BankMore.ContaCorrente.Domain.Entities;


namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly ContaCorrenteDbContext _context;

        public ContaCorrenteRepository(ContaCorrenteDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.ContaCorrente> ObterPorIdAsync(string id)
        {
            return await _context.ContasCorrentes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Domain.Entities.ContaCorrente> ObterPorNumeroAsync(int numero)
        {
            return await _context.ContasCorrentes.FirstOrDefaultAsync(c => c.Numero == numero);
        }

        public async Task<Domain.Entities.ContaCorrente> ObterPorCpfAsync(Cpf cpf)
        {
            // Como CPF está sendo ignorado no EF, precisamos buscar de outra forma
            // Para simplificar, vamos buscar por uma propriedade adicional ou usar query raw
            // Por ora, retorna null - implementação específica dependeria da estratégia escolhida
            return null;
        }

        public async Task<bool> ExistePorCpfAsync(Cpf cpf)
        {
            // Implementação similar ao ObterPorCpfAsync
            return false;
        }

        public async Task<bool> ExistePorNumeroAsync(int numero)
        {
            return await _context.ContasCorrentes.AnyAsync(c => c.Numero == numero);
        }

        public async Task AdicionarAsync(Domain.Entities.ContaCorrente contaCorrente)
        {
            _context.ContasCorrentes.Add(contaCorrente);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Domain.Entities.ContaCorrente contaCorrente)
        {
            _context.ContasCorrentes.Update(contaCorrente);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Movimento>> ObterMovimentosAsync(string idContaCorrente)
        {
            return await _context.Movimentos
                .Where(m => m.IdContaCorrente == idContaCorrente)
                .OrderBy(m => m.DataMovimento)
                .ToListAsync();
        }

        public async Task AdicionarMovimentoAsync(Movimento movimento)
        {
            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync();
        }
    }
}