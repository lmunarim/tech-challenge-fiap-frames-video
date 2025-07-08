using fiap.Domain.Entities;
using fiap.Repositories;
using Moq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Xunit;

namespace fiap.Tests.Repositories
{
    public class ClienteRepositoryTests
    {
        private Cliente cliente;
        private List<Cliente> lstCliente = new List<Cliente>();
        public ClienteRepositoryTests()
        {
            cliente = new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" };
            lstCliente = new List<Cliente> {
                new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" } ,
                 new Cliente { Cpf = "22345678910", Email = "teste2@teste.com", Id = 1, Nome = "Joao2 da Silva" }
            };
        }
        [Fact]
        public void ObterPorIdTest()
        {
            var _repo = new Mock<Func<IDbConnection>>();
            var _logger = new Mock<Serilog.ILogger>();

            var readerMock = new Mock<IDataReader>();

            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);

            readerMock.SetupSequence(reader => reader["IdCliente"]).Returns(1);
            readerMock.SetupSequence(reader => reader["Nome"]).Returns("teste");
            readerMock.SetupSequence(reader => reader["Cpf"]).Returns("123456");
            readerMock.SetupSequence(reader => reader["Email"]).Returns("teste@teste.com");

            var commandMock = new Mock<IDbCommand>();

            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();
            
            parameterMock.SetupSequence(x=>x.ParameterName).Returns("@id");
            parameterMock.SetupSequence(x => x.Value).Returns("1");

            List<DbParameter> lstParameter = new List<DbParameter>();

            SqlCommand cmd = new SqlCommand();
            lstParameter.Add(cmd.CreateParameter());

            commandMock.Setup(m => m.CreateParameter()).Returns(lstParameter[0]);
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupSequence(m => m.CreateCommand()).Returns(commandMock.Object);

            _repo.Setup(a => a.Invoke()).Returns(connectionMock.Object);

            var data = new ClienteRepository(_logger.Object, _repo.Object);

            //Act
            var result = data.Obter(1);

            Assert.NotNull(result);
        }

        [Fact]
        public void ObterTest()
        {
            var _repo = new Mock<Func<IDbConnection>>();
            var _logger = new Mock<Serilog.ILogger>();

            var readerMock = new Mock<IDataReader>();

            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);

            readerMock.SetupSequence(reader => reader["IdCliente"]).Returns(1);
            readerMock.SetupSequence(reader => reader["Nome"]).Returns("teste");
            readerMock.SetupSequence(reader => reader["Cpf"]).Returns("123456");
            readerMock.SetupSequence(reader => reader["Email"]).Returns("teste@teste.com");

            var commandMock = new Mock<IDbCommand>();

            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();

            parameterMock.SetupSequence(x => x.ParameterName).Returns("@id");
            parameterMock.SetupSequence(x => x.Value).Returns("1");

            List<DbParameter> lstParameter = new List<DbParameter>();

            SqlCommand cmd = new SqlCommand();
            lstParameter.Add(cmd.CreateParameter());

            commandMock.Setup(m => m.CreateParameter()).Returns(lstParameter[0]);
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupSequence(m => m.CreateCommand()).Returns(commandMock.Object);

            _repo.Setup(a => a.Invoke()).Returns(connectionMock.Object);

            var data = new ClienteRepository(_logger.Object, _repo.Object);

            //Act
            var result = data.Obter();

            Assert.NotNull(result);
        }

        [Fact]
        public void ObterPrCpf_Test()
        {
            var _repo = new Mock<Func<IDbConnection>>();
            var _logger = new Mock<Serilog.ILogger>();

            var readerMock = new Mock<IDataReader>();

            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);

            readerMock.SetupSequence(reader => reader["IdCliente"]).Returns(1);
            readerMock.SetupSequence(reader => reader["Nome"]).Returns("teste");
            readerMock.SetupSequence(reader => reader["Cpf"]).Returns("123456");
            readerMock.SetupSequence(reader => reader["Email"]).Returns("teste@teste.com");

            var commandMock = new Mock<IDbCommand>();

            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();

            parameterMock.SetupSequence(x => x.ParameterName).Returns("@id");
            parameterMock.SetupSequence(x => x.Value).Returns("1");

            List<DbParameter> lstParameter = new List<DbParameter>();

            SqlCommand cmd = new SqlCommand();
            lstParameter.Add(cmd.CreateParameter());

            commandMock.Setup(m => m.CreateParameter()).Returns(lstParameter[0]);
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupSequence(m => m.CreateCommand()).Returns(commandMock.Object);

            _repo.Setup(a => a.Invoke()).Returns(connectionMock.Object);

            var data = new ClienteRepository(_logger.Object, _repo.Object);

            //Act
            var result = data.ObterPorCpf("123456");

            Assert.NotNull(result);
        }

        [Fact]
        public void Inserir_Test()
        {
            var _repo = new Mock<Func<IDbConnection>>();
            var _logger = new Mock<Serilog.ILogger>();

            var commandMock = new Mock<IDbCommand>();

            commandMock.Setup(m => m.ExecuteNonQuery()).Returns(1).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();

            parameterMock.SetupSequence(x => x.ParameterName).Returns("@id");
            parameterMock.SetupSequence(x => x.Value).Returns("1");

            List<DbParameter> lstParameter = new List<DbParameter>();

            SqlCommand cmd = new SqlCommand();
            lstParameter.Add(cmd.CreateParameter());

            // commandMock.Setup(m => m.CreateParameter()).Returns(lstParameter[0]);
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupSequence(m => m.CreateCommand()).Returns(commandMock.Object);

            _repo.Setup(a => a.Invoke()).Returns(connectionMock.Object);

            var data = new ClienteRepository(_logger.Object, _repo.Object);

            //Act
            var result = data.Inserir(new Cliente { Cpf = "123" , Email = "teste@teste.com", Id = 1 , Nome = "teste" });

            Assert.NotNull(result);
        }

        [Fact]
        public void Atualizar_Test()
        {
            var _repo = new Mock<Func<IDbConnection>>();
            var _logger = new Mock<Serilog.ILogger>();

            var commandMock = new Mock<IDbCommand>();

            commandMock.Setup(m => m.ExecuteNonQuery()).Returns(1).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();

            parameterMock.SetupSequence(x => x.ParameterName).Returns("@id");
            parameterMock.SetupSequence(x => x.Value).Returns("1");

            List<DbParameter> lstParameter = new List<DbParameter>();

            SqlCommand cmd = new SqlCommand();
            lstParameter.Add(cmd.CreateParameter());

            // commandMock.Setup(m => m.CreateParameter()).Returns(lstParameter[0]);
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));
            commandMock.Setup(m => m.Parameters.Add(cmd.CreateParameter()));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupSequence(m => m.CreateCommand()).Returns(commandMock.Object);

            _repo.Setup(a => a.Invoke()).Returns(connectionMock.Object);

            var data = new ClienteRepository(_logger.Object, _repo.Object);

            //Act
            var result = data.Atualizar(new Cliente { Cpf = "123", Email = "teste@teste.com", Id = 1, Nome = "teste" });

            Assert.NotNull(result);
        }
    }
}