using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankMore.Tarifa.Application.Events
{
    public class TransferenciaRealizadaEvent
    {
        public string ChaveIdempotencia { get; set; }
        public string ContaCorrenteId { get; set; }
        public DateTime DataTransferencia { get; set; }
    }
}