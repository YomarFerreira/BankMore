using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.Transferencia.Domain.Repositories;
using BankMore.Transferencia.Infrastructure.Data;


namespace BankMore.Transferencia.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly TransferenciaDbContext _context;

        public IdempotenciaRepository(TransferenciaDbContext context)
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