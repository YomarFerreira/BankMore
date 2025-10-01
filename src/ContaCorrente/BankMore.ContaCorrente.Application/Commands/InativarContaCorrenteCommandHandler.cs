using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.Shared.Infrastructure.Security;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class InativarContaCorrenteCommandHandler : IRequestHandler<InativarContaCorrenteCommand, InativarContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public InativarContaCorrenteCommandHandler(
            IContaCorrenteRepository repository,
            IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<InativarContaCorrenteResponse> Handle(InativarContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contaCorrente = await _repository.ObterPorIdAsync(request.ContaCorrenteId);
                
                if (contaCorrente == null)
                {
                    return new InativarContaCorrenteResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta corrente não encontrada",
                        TipoFalha = TipoFalha.INVALID_ACCOUNT
                    };
                }

                // Verificar senha
                if (!_passwordHasher.VerifyPassword(request.Senha, contaCorrente.Senha, contaCorrente.Salt))
                {
                    return new InativarContaCorrenteResponse
                    {
                        Sucesso = false,
                        Mensagem = "Senha inválida",
                        TipoFalha = TipoFalha.USER_UNAUTHORIZED
                    };
                }

                contaCorrente.Inativar();
                await _repository.AtualizarAsync(contaCorrente);

                return new InativarContaCorrenteResponse
                {
                    Sucesso = true
                };
            }
            catch (Exception ex)
            {
                return new InativarContaCorrenteResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }
    }
}