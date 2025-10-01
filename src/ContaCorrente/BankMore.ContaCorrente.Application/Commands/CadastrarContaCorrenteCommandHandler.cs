using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.ContaCorrente.Domain.Repositories;
using BankMore.Shared.Domain.ValueObjects;
using BankMore.Shared.Infrastructure.Security;
using BankMore.Shared.Domain.Enums;
using MediatR;


namespace BankMore.ContaCorrente.Application.Commands
{
    public class CadastrarContaCorrenteCommandHandler : IRequestHandler<CadastrarContaCorrenteCommand, CadastrarContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public CadastrarContaCorrenteCommandHandler(
            IContaCorrenteRepository repository,
            IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<CadastrarContaCorrenteResponse> Handle(CadastrarContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar CPF
                if (!Cpf.IsValid(request.Cpf))
                {
                    return new CadastrarContaCorrenteResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF inválido",
                        TipoFalha = TipoFalha.INVALID_DOCUMENT
                    };
                }

                var cpf = new Cpf(request.Cpf);

                // Verificar se CPF já existe
                if (await _repository.ExistePorCpfAsync(cpf))
                {
                    return new CadastrarContaCorrenteResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF já cadastrado",
                        TipoFalha = TipoFalha.INVALID_DOCUMENT
                    };
                }

                // Hash da senha
                var (hash, salt) = _passwordHasher.HashPassword(request.Senha);

                // Criar conta corrente
                var contaCorrente = new Domain.Entities.ContaCorrente(request.Nome, cpf, hash, salt);

                // Garantir número único
                while (await _repository.ExistePorNumeroAsync(contaCorrente.Numero))
                {
                    contaCorrente = new Domain.Entities.ContaCorrente(request.Nome, cpf, hash, salt);
                }

                await _repository.AdicionarAsync(contaCorrente);

                return new CadastrarContaCorrenteResponse
                {
                    Sucesso = true,
                    NumeroConta = contaCorrente.Numero
                };
            }
            catch (Exception ex)
            {
                return new CadastrarContaCorrenteResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }
    }
}