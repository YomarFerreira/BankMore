using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Tarifa.Application.Events;
using BankMore.Tarifa.Application.Handlers;
using KafkaFlow;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Tarifa.Infrastructure.Kafka
{
    public class TransferenciaRealizadaConsumer : IMessageHandler<TransferenciaRealizadaEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public TransferenciaRealizadaConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(IMessageContext context, TransferenciaRealizadaEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TransferenciaRealizadaHandler>();
            await handler.Handle(message);
        }
    }
}