using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fiap.Application.UseCases
{
    public class ClienteApplication : IClienteApplication
    {
        private readonly ILogger _logger;
        private readonly IClienteRepository _clienteRepository;
        public ClienteApplication(ILogger logger, IClienteRepository clienteRepository)
        {
            _logger = logger;
            _clienteRepository = clienteRepository;
        }
        public async Task<bool> Inserir(Cliente cliente)
        {
            try
            {
                _logger.Information("Inserindo novo cliente."); ;
                return await _clienteRepository.Inserir(cliente);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao incluir cliente. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Cliente>> Obter()
        {
            try
            {
                _logger.Information("Buscando lista de clientes.");
                return await _clienteRepository.Obter();

            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter clientes. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<Cliente> Obter(int id)
        {
            try
            {
                _logger.Information($"Buscando cliente por id: {id}.");
                return await _clienteRepository.Obter(id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter cliente por id: {id}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<Cliente> ObterPorCpf(string cpf)
        {
            try
            {
                _logger.Information($"Buscando cliente por cpf: {cpf}.");
                return await _clienteRepository.ObterPorCpf(cpf);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter cliente por cpf: {cpf}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> Atualizar(Cliente cliente)
        {
            try
            {
                _logger.Information($"Atualizando cadastro cliente id: {cliente.Id}");
                return await _clienteRepository.Atualizar(cliente);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao atualizar cadastro cliente por cpf: {cliente.Id}. Erro: {ex.Message}");
                throw;
            }
        }
    }
}
