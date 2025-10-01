using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.Transferencia.Domain.Repositories;
using BankMore.Transferencia.Domain.Services;
using BankMore.Shared.Domain.Enums;
using System.Text.Json;


namespace BankMore.Transferencia.Application.Commands
{
    public class EfetuarTransferenciaCommandHandler : IRequestHandler<EfetuarTransferenciaCommand, EfetuarTransferenciaResponse>
    {
        private readonly ITransferenciaRepository _repository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IContaCorrenteService _contaCorrenteService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public EfetuarTransferenciaCommandHandler(
            ITransferenciaRepository repository,
            IIdempotenciaRepository idempotenciaRepository,
            IContaCorrenteService contaCorrenteService,
            IKafkaProducerService kafkaProducerService)
        {
            _repository = repository;
            _idempotenciaRepository = idempotenciaRepository;
            _contaCorrenteService = contaCorrenteService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<EfetuarTransferenciaResponse> Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar idempotência
                var idempotenciaExistente = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
                if (idempotenciaExistente != null)
                {
                    return JsonSerializer.Deserialize<EfetuarTransferenciaResponse>(idempotenciaExistente.Resultado);
                }

                // Validações
                if (request.Valor <= 0)
                {
                    var responseValorInvalido = new EfetuarTransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Valor deve ser positivo",
                        TipoFalha = TipoFalha.INVALID_VALUE
                    };
                    
                    await SalvarIdempotencia(request, responseValorInvalido);
                    return responseValorInvalido;
                }

                // Validar conta corrente de origem
                if (!await _contaCorrenteService.ValidarContaCorrenteAsync(request.ContaCorrenteId, request.Token))
                {
                    var responseContaInvalida = new EfetuarTransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente de origem inválida ou inativa",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                    
                    await SalvarIdempotencia(request, responseContaInvalida);
                    return responseContaInvalida;
                }

                // Efetuar débito na conta de origem
                var debitoSucesso = await _contaCorrenteService.EfetuarMovimentacaoAsync(
                    $"{request.ChaveIdempotencia}_debito", 
                    null, 
                    request.Valor, 
                    'D', 
                    request.Token);

                if (!debitoSucesso)
                {
                    var responseDebitoFalhou = new EfetuarTransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao efetuar débito na conta de origem",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                    
                    await SalvarIdempotencia(request, responseDebitoFalhou);
                    return responseDebitoFalhou;
                }

                // Efetuar crédito na conta de destino
                var creditoSucesso = await _contaCorrenteService.EfetuarMovimentacaoAsync(
                    $"{request.ChaveIdempotencia}_credito", 
                    request.NumeroContaDestino, 
                    request.Valor, 
                    'C', 
                    request.Token);

                if (!creditoSucesso)
                {
                    // Estornar o débito
                    await _contaCorrenteService.EfetuarMovimentacaoAsync(
                        $"{request.ChaveIdempotencia}_estorno", 
                        null, 
                        request.Valor, 
                        'C', 
                        request.Token);

                    var responseCreditoFalhou = new EfetuarTransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao efetuar crédito na conta de destino",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                    
                    await SalvarIdempotencia(request, responseCreditoFalhou);
                    return responseCreditoFalhou;
                }

                // Obter ID da conta de destino (simulação - seria obtido via API)
                var idContaDestino = $"dest_{request.NumeroContaDestino}";

                // Persistir transferência
                var transferencia = new Domain.Entities.Transferencia(
                    request.ContaCorrenteId, 
                    idContaDestino, 
                    request.Valor);

                await _repository.AdicionarAsync(transferencia);

                // Publicar evento no Kafka
                await _kafkaProducerService.PublicarTransferenciaRealizadaAsync(
                    request.ChaveIdempotencia, 
                    request.ContaCorrenteId);

                var responseSucesso = new EfetuarTransferenciaResponse
                {
                    Sucesso = true
                };

                await SalvarIdempotencia(request, responseSucesso);
                return responseSucesso;
            }
            catch (Exception ex)
            {
                var responseErro = new EfetuarTransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };

                await SalvarIdempotencia(request, responseErro);
                return responseErro;
            }
        }

        private async Task SalvarIdempotencia(EfetuarTransferenciaCommand request, EfetuarTransferenciaResponse response)
        {
            try
            {
                var requisicaoJson = JsonSerializer.Serialize(request);
                var resultadoJson = JsonSerializer.Serialize(response);
                
                var idempotencia = new Domain.Entities.Idempotencia(
                    request.ChaveIdempotencia,
                    requisicaoJson,
                    resultadoJson);

                await _idempotenciaRepository.AdicionarAsync(idempotencia);
            }
            catch
            {
                // Log do erro, mas não falhar a operação principal
            }
        }
    }
}