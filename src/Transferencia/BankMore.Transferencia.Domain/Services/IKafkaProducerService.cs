using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankMore.Transferencia.Domain.Services
{
    public interface IKafkaProducerService
    {
        Task PublicarTransferenciaRealizadaAsync(string chaveIdempotencia, string contaCorrenteId);
    }
}