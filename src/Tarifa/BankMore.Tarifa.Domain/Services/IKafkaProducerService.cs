using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMore.Tarifa.Domain.Services
{
    public interface IKafkaProducerService
    {
        Task PublicarTarifacaoRealizadaAsync(string contaCorrenteId, decimal valor);
    }
}