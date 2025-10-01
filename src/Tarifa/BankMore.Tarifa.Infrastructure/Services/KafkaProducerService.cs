using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Tarifa.Domain.Services;
using KafkaFlow;
using KafkaFlow.Producers;


namespace BankMore.Tarifa.Infrastructure.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IMessageProducer<TarifacaoRealizadaProducer> _producer;

        public KafkaProducerService(IMessageProducer<TarifacaoRealizadaProducer> producer)
        {
            _producer = producer;
        }

        public async Task PublicarTarifacaoRealizadaAsync(string contaCorrenteId, decimal valor)
        {
            try
            {
                var evento = new
                {
                    ContaCorrenteId = contaCorrenteId,
                    Valor = valor,
                    DataTarifacao = DateTime.Now
                };

                // Executa com timeout de 3 segundos para não travar a API
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await _producer.ProduceAsync(contaCorrenteId, evento).WaitAsync(cts.Token);

                Console.WriteLine($"Tarifação publicada no Kafka com sucesso: {contaCorrenteId}");
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"AVISO: Timeout ao publicar tarifação no Kafka (Kafka indisponível?). Tarifa registrada, mas evento não publicado.");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"AVISO: Timeout ao publicar tarifação no Kafka (Kafka indisponível?). Tarifa registrada, mas evento não publicado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AVISO: Erro ao publicar tarifação no Kafka: {ex.Message}. Tarifa registrada, mas evento não publicado.");
            }
        }
    }

    public class TarifacaoRealizadaProducer { }
}