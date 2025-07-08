using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace fiap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly IClienteApplication _clienteApplication;
        public ClienteController(Serilog.ILogger logger, IClienteApplication clienteApplication)
        {
            _logger = logger;
            _clienteApplication = clienteApplication;
        }

        /// <summary>
        /// Buscar clientes
        /// </summary>
        /// <returns>Lista de clientes cadastrados</returns>
        /// <response code = "200">Retorna a lista de clientes cadastrados</response>
        /// <response code = "400">Se houver erro na busca por clientes</response>
        /// <response code = "500">Se houver erro de conexão com banco de dados</response>
        // GET: api/<ClienteController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.Information("Buscando lista de clientes");
                return Ok(await _clienteApplication.Obter());
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter clientes. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Buscar cliente por id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Cliente por id</returns>
        /// <response code = "200">Retorna a busca do cliente por id se existir</response>
        /// <response code = "400">Se o id do cliente não existir</response>
        /// <response code = "500">Se houver erro de conexão com banco de dados</response>        
        // GET api/<ClienteController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                _logger.Information($"Buscando cliente por id: {id}");
                return Ok(await _clienteApplication.Obter(id));

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter cliente id: {id}. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Buscar cliente por cpf
        /// </summary>
        /// <param name="cpf">CPF do cliente</param>
        /// <returns>Cliente por cpf</returns>
        /// <response code = "200">Retorna a busca do cliente por cpf se existir</response>
        /// <response code = "400">Se o cpf do cliente não existir</response>
        /// <response code = "500">Se houver erro de conexão com banco de dados</response>  
        [HttpGet("ObterPorCpf/{cpf}")]
        public async Task<IActionResult> ObterPorCpf(string cpf)
        {
            try
            {
                _logger.Information($"Buscando cadastro cliente por cpf: {cpf}.");
                return Ok(await _clienteApplication.ObterPorCpf(cpf));
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter cliente por cpf: {cpf}. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Cadastrar cliente
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///     POST /Cliente
        ///     {
        ///         "nome": "Maria",
        ///         "cpf": "12345678910",
        ///         "email": "teste@yahoo.com.br"   
        ///     }
        /// 
        /// </remarks>
        /// <param name="cliente"></param>
        /// <returns>Um novo cliente cadastrado</returns>
        /// <response code = "200">Retorna o novo cliente cadastrado</response>
        /// <response code = "400">Se o cliente não for cadastrado</response>
        /// <response code = "500">Se houver erro de conexão com banco de dados</response>        
        // POST api/<ClienteController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            try
            {
                _logger.Information("Inserindo novo cliente.");
                if (await _clienteApplication.Inserir(cliente))
                    return Ok(new { Mensagem = "Cliente incluido com sucesso!" });

                return BadRequest(new { Mensagem = "Erro ao incluir cliente!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao incluir cliente. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Alterar cadastro cliente
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///     PUT /Cliente
        ///     {
        ///         "id": 1,
        ///         "nome": "Maria Silva",
        ///         "cpf": "12345678910",
        ///         "email": "teste2@yahoo.com.br"      
        ///     }
        ///     
        /// </remarks>
        /// <param name="cliente"></param>
        /// <returns>Cadstro cliente alterado</returns>
        /// <response code = "200">Retorna o cadastrado do cliente alterado</response>
        /// <response code = "400">Se houver erro ao alterar o cadastro do cliente</response>
        /// <response code = "500">Se houver erro de conexão com banco de dados</response> 
        // PUT api/<ClienteController>/5
        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] Cliente cliente)
        {
            try
            {
                _logger.Information($"Atualizando cadastro cliente id: {cliente.Id}.");
                if (await _clienteApplication.Atualizar(cliente))
                    return Ok(new { Mensagem = "Cliente alterado com sucesso!" });

                return BadRequest(new { Mensagem = "Erro ao alterar cliente!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao alterar cliente. Erro: {ex.Message}");
            }
        }
    }
}
