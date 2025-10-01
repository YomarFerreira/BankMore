using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Transferencia.Domain.Services;
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.Transferencia.Infrastructure.Services

{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IMessageProducer<TransferenciaRealizadaProducer> _producer;

        public KafkaProducerService(IMessageProducer<TransferenciaRealizadaProducer> producer)
        {
            _producer = producer;
        }

        public async Task PublicarTransferenciaRealizadaAsync(string chaveIdempotencia, string contaCorrenteId)
        {
            try
            {
                var evento = new
                {
                    ChaveIdempotencia = chaveIdempotencia,
                    ContaCorrenteId = contaCorrenteId,
                    DataTransferencia = DateTime.Now
                };

                // Executa com timeout de 3 segundos para não travar a API
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await _producer.ProduceAsync(contaCorrenteId, evento).WaitAsync(cts.Token);

                Console.WriteLine($"Evento publicado no Kafka com sucesso: {chaveIdempotencia}");
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"AVISO: Timeout ao publicar no Kafka (Kafka indisponível?). Transferência registrada, mas evento não publicado.");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"AVISO: Timeout ao publicar no Kafka (Kafka indisponível?). Transferência registrada, mas evento não publicado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AVISO: Erro ao publicar no Kafka: {ex.Message}. Transferência registrada, mas evento não publicado.");
            }
        }
    }

    public class TransferenciaRealizadaProducer { }
}