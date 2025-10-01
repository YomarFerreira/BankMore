using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.Shared.Domain.Enums;
using System.Text.Json;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class MovimentacaoCommandHandler : IRequestHandler<MovimentacaoCommand, MovimentacaoResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentacaoCommandHandler(
            IContaCorrenteRepository repository,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _repository = repository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<MovimentacaoResponse> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar idempotência
                var idempotenciaExistente = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
                if (idempotenciaExistente != null)
                {
                    return JsonSerializer.Deserialize<MovimentacaoResponse>(idempotenciaExistente.Resultado);
                }

                // Validações
                if (request.Valor <= 0)
                {
                    var responseValorInvalido = new MovimentacaoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Valor deve ser positivo",
                        TipoFalha = TipoFalha.INVALID_VALUE
                    };
                    
                    await SalvarIdempotencia(request, responseValorInvalido);
                    return responseValorInvalido;
                }

                if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
                {
                    var responseTipoInvalido = new MovimentacaoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Tipo de movimento inválido",
                        TipoFalha = TipoFalha.INVALID_TYPE
                    };
                    
                    await SalvarIdempotencia(request, responseTipoInvalido);
                    return responseTipoInvalido;
                }

                // Determinar conta corrente a ser movimentada
                string contaCorrenteIdParaMovimentar = request.ContaCorrenteId;
                
                if (request.NumeroContaCorrente.HasValue)
                {
                    var contaPorNumero = await _repository.ObterPorNumeroAsync(request.NumeroContaCorrente.Value);
                    if (contaPorNumero == null)
                    {
                        var responseContaInvalida = new MovimentacaoResponse
                        {
                            Sucesso = false,
                            Mensagem = "Conta corrente não encontrada",
                            TipoFalha = TipoFalha.INVALID_ACCOUNT
                        };
                        
                        await SalvarIdempotencia(request, responseContaInvalida);
                        return responseContaInvalida;
                    }
                    
                    contaCorrenteIdParaMovimentar = contaPorNumero.Id;
                    
                    // Validar se é crédito quando conta é diferente do usuário logado
                    if (contaCorrenteIdParaMovimentar != request.ContaCorrenteId && request.TipoMovimento != 'C')
                    {
                        var responseTipoInvalidoOutraConta = new MovimentacaoResponse
                        {
                            Sucesso = false,
                            Mensagem = "Apenas créditos são permitidos em contas de terceiros",
                            TipoFalha = TipoFalha.INVALID_TYPE
                        };
                        
                        await SalvarIdempotencia(request, responseTipoInvalidoOutraConta);
                        return responseTipoInvalidoOutraConta;
                    }
                }

                var contaCorrente = await _repository.ObterPorIdAsync(contaCorrenteIdParaMovimentar);
                
                if (contaCorrente == null)
                {
                    var responseContaInvalida = new MovimentacaoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente não encontrada",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                    
                    await SalvarIdempotencia(request, responseContaInvalida);
                    return responseContaInvalida;
                }

                if (!contaCorrente.Ativo)
                {
                    var responseContaInativa = new MovimentacaoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente inativa",
                        TipoFalha = TipoFalha.INACTIVE_ACCOUNT
                    };
                    
                    await SalvarIdempotencia(request, responseContaInativa);
                    return responseContaInativa;
                }

                // Criar movimento
                var tipoMovimento = request.TipoMovimento == 'C' ? TipoMovimento.Credito : TipoMovimento.Debito;
                var movimento = new Domain.Entities.Movimento(contaCorrenteIdParaMovimentar, tipoMovimento, request.Valor);

                await _repository.AdicionarMovimentoAsync(movimento);

                var responseSucesso = new MovimentacaoResponse
                {
                    Sucesso = true
                };

                await SalvarIdempotencia(request, responseSucesso);
                return responseSucesso;
            }
            catch (Exception ex)
            {
                var responseErro = new MovimentacaoResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };

                await SalvarIdempotencia(request, responseErro);
                return responseErro;
            }
        }

        private async Task SalvarIdempotencia(MovimentacaoCommand request, MovimentacaoResponse response)
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