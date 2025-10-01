using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.Shared.Infrastructure.Security;
using BankMore.Shared.Domain.Enums;
using BankMore.ContaCorrente.API.Models;


namespace BankMore.ContaCorrente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;

        public ContaCorrenteController(IMediator mediator, IJwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Cadastra uma nova conta corrente
        /// </summary>
        /// <param name="request">Dados do cadastro</param>
        /// <returns>Número da conta criada</returns>
        /// <response code="200">Conta criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost("cadastrar")]
        public async Task<IActionResult> CadastrarContaCorrente([FromBody] CadastrarContaCorrenteRequest request)
        {
            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = request.Cpf,
                Nome = request.Nome,
                Senha = request.Senha
            };

            var response = await _mediator.Send(command);

            if (!response.Sucesso)
            {
                return BadRequest(new
                {
                    mensagem = response.Mensagem,
                    tipo = response.TipoFalha?.ToString()
                });
            }

            return Ok(new { numeroConta = response.NumeroConta });
        }

        /// <summary>
        /// Efetua login na conta corrente
        /// </summary>
        /// <param name="request">Credenciais de login</param>
        /// <returns>Token JWT</returns>
        /// <response code="200">Login efetuado com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand
            {
                NumeroContaOuCpf = request.NumeroContaOuCpf,
                Senha = request.Senha
            };

            var response = await _mediator.Send(command);

            if (!response.Sucesso)
            {
                return Unauthorized(new
                {
                    mensagem = response.Mensagem,
                    tipo = response.TipoFalha?.ToString()
                });
            }

            return Ok(new { token = response.Token });
        }

        /// <summary>
        /// Inativa a conta corrente
        /// </summary>
        /// <param name="request">Senha para confirmação</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Conta inativada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Token inválido</response>
        [HttpPost("inativar")]
        [Authorize]
        public async Task<IActionResult> InativarContaCorrente([FromBody] InativarContaCorrenteRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var contaCorrenteId = _jwtService.GetContaCorrenteIdFromToken(token);

            if (string.IsNullOrEmpty(contaCorrenteId))
            {
                return Forbid();
            }

            var command = new InativarContaCorrenteCommand
            {
                ContaCorrenteId = contaCorrenteId,
                Senha = request.Senha
            };

            var response = await _mediator.Send(command);

            if (!response.Sucesso)
            {
                return BadRequest(new
                {
                    mensagem = response.Mensagem,
                    tipo = response.TipoFalha?.ToString()
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Efetua movimentação na conta corrente
        /// </summary>
        /// <param name="request">Dados da movimentação</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Movimentação efetuada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Token inválido</response>
        [HttpPost("movimentacao")]
        [Authorize]
        public async Task<IActionResult> Movimentacao([FromBody] MovimentacaoRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var contaCorrenteId = _jwtService.GetContaCorrenteIdFromToken(token);

            if (string.IsNullOrEmpty(contaCorrenteId))
            {
                return Forbid();
            }

            var command = new MovimentacaoCommand
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                ContaCorrenteId = contaCorrenteId,
                NumeroContaCorrente = request.NumeroContaCorrente,
                Valor = request.Valor,
                TipoMovimento = request.TipoMovimento
            };

            var response = await _mediator.Send(command);

            if (!response.Sucesso)
            {
                return BadRequest(new
                {
                    mensagem = response.Mensagem,
                    tipo = response.TipoFalha?.ToString()
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Consulta o saldo da conta corrente
        /// </summary>
        /// <returns>Dados do saldo</returns>
        /// <response code="200">Saldo consultado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Token inválido</response>
        [HttpGet("saldo")]
        [Authorize]
        public async Task<IActionResult> ConsultarSaldo()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var contaCorrenteId = _jwtService.GetContaCorrenteIdFromToken(token);

            if (string.IsNullOrEmpty(contaCorrenteId))
            {
                return Forbid();
            }

            var query = new ConsultarSaldoQuery
            {
                ContaCorrenteId = contaCorrenteId
            };

            var response = await _mediator.Send(query);

            if (!response.Sucesso)
            {
                return BadRequest(new
                {
                    mensagem = response.Mensagem,
                    tipo = response.TipoFalha?.ToString()
                });
            }

            return Ok(new
            {
                numeroContaCorrente = response.NumeroContaCorrente,
                nomeTitular = response.NomeTitular,
                dataConsulta = response.DataConsulta?.ToString("dd/MM/yyyy HH:mm:ss"),
                saldo = response.Saldo?.ToString("F2")
            });
        }
    }
}