using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMore.Transferencia.Domain.Services;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;


namespace BankMore.Transferencia.Infrastructure.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ContaCorrenteService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> ValidarContaCorrenteAsync(string contaCorrenteId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.GetAsync($"{_configuration["ContaCorrenteApi:BaseUrl"]}/api/contacorrente/saldo");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EfetuarMovimentacaoAsync(string chaveIdempotencia, int? numeroContaCorrente, decimal valor, char tipoMovimento, string token)
        {
            try
            {
                var request = new
                {
                    ChaveIdempotencia = chaveIdempotencia,
                    NumeroContaCorrente = numeroContaCorrente,
                    Valor = valor,
                    TipoMovimento = tipoMovimento
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.PostAsync($"{_configuration["ContaCorrenteApi:BaseUrl"]}/api/contacorrente/movimentacao", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}