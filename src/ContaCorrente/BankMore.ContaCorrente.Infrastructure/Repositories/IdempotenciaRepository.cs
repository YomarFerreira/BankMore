using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.ContaCorrente.Infrastructure.Data;


namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly ContaCorrenteDbContext _context;

        public IdempotenciaRepository(ContaCorrenteDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Idempotencia> ObterPorChaveAsync(string chave)
        {
            return await _context.Idempotencias.FirstOrDefaultAsync(i => i.ChaveIdempotencia == chave);
        }

        public async Task AdicionarAsync(Domain.Entities.Idempotencia idempotencia)
        {
            _context.Idempotencias.Add(idempotencia);
            await _context.SaveChangesAsync();
        }
    }
}