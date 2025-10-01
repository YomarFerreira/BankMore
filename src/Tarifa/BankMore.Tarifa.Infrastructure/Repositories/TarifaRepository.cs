using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMore.Tarifa.Domain.Repositories;
using BankMore.Tarifa.Infrastructure.Data;


namespace BankMore.Tarifa.Infrastructure.Repositories
{
    public class TarifaRepository : ITarifaRepository
    {
        private readonly TarifaDbContext _context;

        public TarifaRepository(TarifaDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Domain.Entities.Tarifa tarifa)
        {
            _context.Tarifas.Add(tarifa);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Domain.Entities.Tarifa>> ObterPorContaCorrenteAsync(string idContaCorrente)
        {
            return await _context.Tarifas
                .Where(t => t.IdContaCorrente == idContaCorrente)
                .OrderByDescending(t => t.DataMovimento)
                .ToListAsync();
        }
    }
}