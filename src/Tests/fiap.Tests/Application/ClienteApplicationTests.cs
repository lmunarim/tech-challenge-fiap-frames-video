using fiap.Application.UseCases;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using Moq;
using Xunit;

namespace fiap.Tests.Application
{
    public class ClienteApplicationTests
    {
        private Cliente cliente;
        private List<Cliente> lstCliente = new List<Cliente>();
        public ClienteApplicationTests()
        {
            cliente = new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" };
            lstCliente = new List<Cliente> {
                new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" } ,
                 new Cliente { Cpf = "22345678910", Email = "teste2@teste.com", Id = 1, Nome = "Joao2 da Silva" }
            };
        }
        [Fact]
        public async Task Obter_OkAsync()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Obter())
                .ReturnsAsync(lstCliente);

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var result = await app.Obter();

            Assert.NotNull(result);
        }
        [Fact]
        public async Task Obter_ComFalha()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Obter())
                 .ThrowsAsync(new System.Exception("Erro"));

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var ex = await Assert.ThrowsAsync<System.Exception>(() => app.Obter());

            Assert.Equal("Erro", ex.Message);
        }
        [Fact]
        public async Task ObterPorId_OkAsync()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Obter(1))
                .ReturnsAsync(cliente);

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var result = await app.Obter(1);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task ObterPorId_ComFalha()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Obter(1))
                 .ThrowsAsync(new System.Exception("Erro"));

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var ex = await Assert.ThrowsAsync<System.Exception>(() => app.Obter(1));

            Assert.Equal("Erro", ex.Message);
        }
        [Fact]
        public async Task ObterPorCpf_OkAsync()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.ObterPorCpf(It.IsAny<string>()))
                .ReturnsAsync(cliente);

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var result = await app.ObterPorCpf("123");

            Assert.NotNull(result);
        }
        [Fact]
        public async Task ObterPorCpf_Comfalha()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.ObterPorCpf(It.IsAny<string>()))
                 .ThrowsAsync(new System.Exception("Erro"));

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var ex = await Assert.ThrowsAsync<System.Exception>(() => app.ObterPorCpf("123"));

            Assert.Equal("Erro", ex.Message);
        }
        [Fact]
        public async Task Inserir_OkAsync()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Inserir(cliente))
                .ReturnsAsync(true);

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var result = await app.Inserir(cliente);

            Assert.True(result);
        }
        [Fact]
        public async Task Inserir_Comfalha()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Inserir(cliente)).ThrowsAsync(new System.Exception("Erro"));

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var ex = await Assert.ThrowsAsync<System.Exception>(() => app.Inserir(cliente));

            Assert.Equal("Erro", ex.Message);
        }
        [Fact]
        public async Task Atualizar_OkAsync()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Atualizar(cliente))
                .ReturnsAsync(true);

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var result = await app.Atualizar(cliente);

            Assert.True(result);
        }
        [Fact]
        public async Task Atualizar_Comfalha()
        {
            var _repo = new Mock<IClienteRepository>();
            var _logger = new Mock<Serilog.ILogger>();

            _repo.SetupSequence(x => x.Atualizar(cliente))
                .ThrowsAsync(new System.Exception("Erro"));

            ClienteApplication app = new(_logger.Object, _repo.Object);
            var ex = await Assert.ThrowsAsync<System.Exception>(() => app.Atualizar(cliente));

            Assert.Equal("Erro", ex.Message);
        }
    }
}
