using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BankMore.ContaCorrente.API.Models
{
    public class CadastrarContaCorrenteRequest
    {
        [Required(ErrorMessage = "CPF é obrigatório")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Número da conta ou CPF é obrigatório")]
        public string NumeroContaOuCpf { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
    }

    public class InativarContaCorrenteRequest
    {
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
    }

    public class MovimentacaoRequest
    {
        [Required(ErrorMessage = "Chave de idempotência é obrigatória")]
        public string ChaveIdempotencia { get; set; }

        public int? NumeroContaCorrente { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Tipo de movimento é obrigatório")]
        public char TipoMovimento { get; set; }
    }
}