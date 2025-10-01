using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.Shared.Domain.ValueObjects;
using BankMore.Shared.Infrastructure.Security;
using BankMore.Shared.Domain.Enums;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(
            IContaCorrenteRepository repository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Domain.Entities.ContaCorrente contaCorrente = null;

                // Tentar buscar por número ou CPF
                if (int.TryParse(request.NumeroContaOuCpf, out int numero))
                {
                    contaCorrente = await _repository.ObterPorNumeroAsync(numero);
                }
                else if (Cpf.IsValid(request.NumeroContaOuCpf))
                {
                    var cpf = new Cpf(request.NumeroContaOuCpf);
                    contaCorrente = await _repository.ObterPorCpfAsync(cpf);
                }

                if (contaCorrente == null)
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Credenciais inválidas",
                        TipoFalha = TipoFalha.USER_UNAUTHORIZED
                    };
                }

                // Verificar senha
                if (!_passwordHasher.VerifyPassword(request.Senha, contaCorrente.Senha, contaCorrente.Salt))
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Credenciais inválidas",
                        TipoFalha = TipoFalha.USER_UNAUTHORIZED
                    };
                }

                // Gerar token
                var token = _jwtService.GenerateToken(contaCorrente.Id, contaCorrente.Numero.ToString());

                return new LoginResponse
                {
                    Sucesso = true,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    TipoFalha = TipoFalha.USER_UNAUTHORIZED
                };
            }
        }
    }
}