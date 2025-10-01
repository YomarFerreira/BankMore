using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using BankMore.Transferencia.Application.Commands;
using BankMore.Shared.Infrastructure.Security;
using System.ComponentModel.DataAnnotations;


namespace BankMore.Transferencia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;

        public TransferenciaController(IMediator mediator, IJwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Efetua transferência entre contas da mesma instituição
        /// </summary>
        /// <param name="request">Dados da transferência</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Transferência efetuada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Token inválido</response>
        [HttpPost]
        public async Task<IActionResult> EfetuarTransferencia([FromBody] EfetuarTransferenciaRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var contaCorrenteId = _jwtService.GetContaCorrenteIdFromToken(token);

            if (string.IsNullOrEmpty(contaCorrenteId))
            {
                return Forbid();
            }

            var command = new EfetuarTransferenciaCommand
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                ContaCorrenteId = contaCorrenteId,
                NumeroContaDestino = request.NumeroContaDestino,
                Valor = request.Valor,
                Token = token
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
    }

    public class EfetuarTransferenciaRequest
    {
        [Required(ErrorMessage = "Chave de idempotência é obrigatória")]
        public string ChaveIdempotencia { get; set; }

        [Required(ErrorMessage = "Número da conta de destino é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Número da conta de destino deve ser válido")]
        public int NumeroContaDestino { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal Valor { get; set; }
    }
}