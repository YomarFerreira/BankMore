using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.Transferencia.Domain.Repositories;
using BankMore.Transferencia.Infrastructure.Data;

namespace BankMore.Transferencia.Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly TransferenciaDbContext _context;

        public TransferenciaRepository(TransferenciaDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Domain.Entities.Transferencia transferencia)
        {
            _context.Transferencias.Add(transferencia);
            await _context.SaveChangesAsync();
        }

        public async Task<Domain.Entities.Transferencia> ObterPorIdAsync(string id)
        {
            return await _context.Transferencias.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Domain.Entities.Transferencia>> ObterPorContaCorrenteAsync(string idContaCorrente)
        {
            return await _context.Transferencias
                .Where(t => t.IdContaCorrenteOrigem == idContaCorrente || t.IdContaCorrenteDestino == idContaCorrente)
                .OrderByDescending(t => t.DataMovimento)
                .ToListAsync();
        }
    }
}