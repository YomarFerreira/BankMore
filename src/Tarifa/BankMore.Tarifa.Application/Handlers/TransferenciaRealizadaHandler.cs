using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Tarifa.Application.Events;
using BankMore.Tarifa.Domain.Repositories;
using BankMore.Tarifa.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BankMore.Tarifa.Application.Handlers
{
    public class TransferenciaRealizadaHandler
    {
        private readonly ITarifaRepository _repository;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransferenciaRealizadaHandler> _logger;

        public TransferenciaRealizadaHandler(
            ITarifaRepository repository,
            IKafkaProducerService kafkaProducerService,
            IConfiguration configuration,
            ILogger<TransferenciaRealizadaHandler> logger)
        {
            _repository = repository;
            _kafkaProducerService = kafkaProducerService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(TransferenciaRealizadaEvent evento)
        {
            try
            {
                _logger.LogInformation($"Processando tarifação para transferência: {evento.ChaveIdempotencia}");

                // Obter valor da tarifa do appsettings
                var valorTarifa = _configuration.GetValue<decimal>("Tarifa:ValorTransferencia");

                // Registrar tarifa no banco
                var tarifa = new Domain.Entities.Tarifa(evento.ContaCorrenteId, valorTarifa);
                await _repository.AdicionarAsync(tarifa);

                // Publicar evento de tarifação realizada
                await _kafkaProducerService.PublicarTarifacaoRealizadaAsync(
                    evento.ContaCorrenteId, 
                    valorTarifa);

                _logger.LogInformation($"Tarifação processada com sucesso para conta: {evento.ContaCorrenteId}, valor: {valorTarifa}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar tarifação para transferência: {evento.ChaveIdempotencia}");
            }
        }
    }
}