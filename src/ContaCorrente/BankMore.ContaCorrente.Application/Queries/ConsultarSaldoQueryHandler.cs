using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Queries
{
    public class ConsultarSaldoQueryHandler : IRequestHandler<ConsultarSaldoQuery, ConsultarSaldoResponse>
    {
        private readonly IContaCorrenteRepository _repository;

        public ConsultarSaldoQueryHandler(IContaCorrenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConsultarSaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var contaCorrente = await _repository.ObterPorIdAsync(request.ContaCorrenteId);
                
                if (contaCorrente == null)
                {
                    return new ConsultarSaldoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente não encontrada",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                }

                if (!contaCorrente.Ativo)
                {
                    return new ConsultarSaldoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente inativa",
                        TipoFalha = TipoFalha.INACTIVE_ACCOUNT
                    };
                }

                // Carregar movimentos para calcular saldo
                var movimentos = await _repository.ObterMovimentosAsync(contaCorrente.Id);
                foreach (var movimento in movimentos)
                {
                    contaCorrente.AdicionarMovimento(movimento);
                }

                var saldo = contaCorrente.CalcularSaldo();

                return new ConsultarSaldoResponse
                {
                    Sucesso = true,
                    NumeroContaCorrente = contaCorrente.Numero,
                    NomeTitular = contaCorrente.Nome,
                    DataConsulta = DateTime.Now,
                    Saldo = saldo
                };
            }
            catch (Exception ex)
            {
                return new ConsultarSaldoResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }
    }
}