using fiap.API.Controllers;
using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using Moq;
using Xunit;

namespace fiap.Tests.ControllerTests
{
    public class ControllerClienteTests
    {
        private Cliente cliente;
        private List<Cliente> lstCliente = new List<Cliente>();
        public ControllerClienteTests()
        {
            cliente = new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" };
            lstCliente = new List<Cliente> { 
                new Cliente { Cpf = "12345678910", Email = "teste@teste.com", Id = 1, Nome = "Joao da Silva" } ,
                 new Cliente { Cpf = "22345678910", Email = "teste2@teste.com", Id = 1, Nome = "Joao2 da Silva" }
            };
        }

        [Fact]
        public async Task Get_OKAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.SetupSequence(x => x.Obter()).ReturnsAsync(lstCliente);

            ClienteController clienteController = new(_logger.Object , _clienteApplication.Object);
            var result = await clienteController.Get(1);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task Get_ComErroAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.SetupSequence(x => x.Obter())
                /// .ReturnsAsync(null)
                .ThrowsAsync(new AggregateException("Erro ao consultar cliente"));

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);

            var result = await clienteController.Get();

            Assert.NotNull(result);
            /// await Assert.ThrowsAsync<AggregateException>(async () => await clienteController.Get());
        }
        [Fact]
        public async Task Get_By_Id_OKAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Obter(It.IsAny<int>())).ReturnsAsync(cliente);

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Get(1);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task Get_By_Id_ExceptionAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Obter(It.IsAny<int>()))
                .ThrowsAsync(new AggregateException("__"));

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Get(1);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task ObterPorCpf_OKAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.ObterPorCpf(It.IsAny<string>())).ReturnsAsync(cliente);

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.ObterPorCpf("123");

            Assert.NotNull(result);
        }
        [Fact]
        public async Task ObterPorCpf_ExceptionAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.ObterPorCpf(It.IsAny<string>()))
                .ThrowsAsync(new AggregateException("__"));

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.ObterPorCpf("123");

            Assert.NotNull(result);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task POST_OKAsync(bool retorno)
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Inserir(It.IsAny<Cliente>())).ReturnsAsync(retorno);

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Post(cliente);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task POST_ExceptionAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Inserir(It.IsAny<Cliente>()))
                .ThrowsAsync(new AggregateException("__"));

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Post(cliente);

            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task PUT_OKAsync(bool retorno)
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Atualizar(It.IsAny<Cliente>())).ReturnsAsync(retorno);

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Put(cliente);

            Assert.NotNull(result);
        }
        [Fact]
        public async Task PUT_ExceptionAsync()
        {
            var _clienteApplication = new Mock<IClienteApplication>();
            var _logger = new Mock<Serilog.ILogger>();

            _clienteApplication.Setup(x => x.Atualizar(It.IsAny<Cliente>()))
                .ThrowsAsync(new AggregateException("__"));

            ClienteController clienteController = new(_logger.Object, _clienteApplication.Object);
            var result = await clienteController.Put(cliente);

            Assert.NotNull(result);
        }
    }
}
