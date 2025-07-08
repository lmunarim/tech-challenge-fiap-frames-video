using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace fiap.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly ILogger _logger;

        public ClienteRepository(ILogger logger, Func<IDbConnection> connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public Task<Cliente> Obter(int id)
        {
            try
            {
                using var connection = _connectionFactory();
                connection.Open();
                _logger.Information("Conexão com o banco de dados realizada com sucesso!");
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Cliente WHERE IdCliente = @id";
                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    _logger.Information($"Obtendo cadastro do cliente por id: {id}.");
                    return Task.FromResult(new Cliente
                    {
                        Id = (int)reader["IdCliente"],
                        Nome = reader["Nome"].ToString(),
                        Cpf = reader["Cpf"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }
                else
                {
                    throw new Exception($"Id cliente {id} não encontrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter cliente por id {id}. Erro: {ex.Message}.");
                throw;
            }
        }

        public Task<bool> Inserir(Cliente cliente)
        {
            try
            {
                using var connection = _connectionFactory();
                connection.Open();
                _logger.Information("Conexão com o banco de dados realizada com sucesso!");

                using var command = connection.CreateCommand();
                command.CommandText = "insert into Cliente values (@nome, @cpf, @email, @data)";

                command.Parameters.Add(new SqlParameter { ParameterName = "@nome", Value = cliente.Nome, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@cpf", Value = cliente.Cpf, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@email", Value = cliente.Email, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@data", Value = DateTime.Now, SqlDbType = SqlDbType.DateTime });

                _logger.Information("Cliente cadastrado com sucesso!");

                return Task.FromResult(command.ExecuteNonQuery() >= 1);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao incluir cliente. Erro: {ex.Message}.");
                return Task.FromResult(false);
            }

        }
        public Task<bool> Atualizar(Cliente cliente)
        {
            try
            {
                using var connection = _connectionFactory();
                StringBuilder sb = new StringBuilder();
                connection.Open();
                _logger.Information("Conexão com o banco de dados realizada com sucesso!");

                using var command = connection.CreateCommand();
                sb.Append("update Cliente set Nome = @nome,");
                sb.Append("Cpf = @cpf, Email = @email ");
                sb.Append(" where IdCliente = @id");

                command.CommandText = sb.ToString();

                command.Parameters.Add(new SqlParameter { ParameterName = "@nome", Value = cliente.Nome, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@cpf", Value = cliente.Cpf, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@email", Value = cliente.Email, SqlDbType = SqlDbType.VarChar });
                command.Parameters.Add(new SqlParameter { ParameterName = "@id", Value = cliente.Id, SqlDbType = SqlDbType.Int });

                _logger.Information($"Cadastro cliente id: {cliente.Id} atualizado com sucesso!");

                return Task.FromResult(command.ExecuteNonQuery() >= 1);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao atualizar cadastro cliente id: {cliente.Id}. Erro: {ex.Message}");
                Task.FromResult(false);
                throw;
            }

        }
        public Task<List<Cliente>> Obter()
        {
            try
            {
                using var connection = _connectionFactory();
                connection.Open();
                _logger.Information("Conexão com o banco de dados realizada com sucesso!");

                var lst = new List<Cliente>();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Cliente";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lst.Add(new Cliente
                    {
                        Id = (int)reader["IdCliente"],
                        Nome = reader["Nome"].ToString(),
                        Cpf = reader["Cpf"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }
                _logger.Information("Retornando lista com os clientes encontrados.");
                return Task.FromResult(lst);
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter lista de clientes. Erro: {ex.Message}.");
                throw;
            }
        }

        public Task<Cliente> ObterPorCpf(string cpf)
        {
            try
            {
                using var connection = _connectionFactory();
                connection.Open();
                _logger.Information("Conexão com o banco de dados realizada com sucesso");

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Cliente WHERE Cpf = @cpf";
                var param = command.CreateParameter();
                param.ParameterName = "@cpf";
                param.Value = cpf;
                command.Parameters.Add(param);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    _logger.Information($"Cadastro cliente cpf: {cpf} obtido com sucesso!");
                    return Task.FromResult(new Cliente
                    {
                        Id = (int)reader["IdCliente"],
                        Nome = reader["Nome"].ToString(),
                        Cpf = reader["Cpf"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }
                else
                {
                    throw new Exception($"Cpf {cpf} não cadastrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter cliente por cpf: {cpf}. Erro: {ex.Message}");
                throw;
            }
        }
    }

}
