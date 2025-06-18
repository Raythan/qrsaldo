using Microsoft.AspNetCore.Mvc;
using QRSaldo.API.DTOs;
using QRSaldo.API.Services;

namespace QRSaldo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Criar novo usuário
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResultadoOperacao<UsuarioDto>>> CriarUsuario(CriarUsuarioDto dto)
        {
            var resultado = await _usuarioService.CriarUsuarioAsync(dto);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = resultado.Dados!.Id }, resultado);
        }

        /// <summary>
        /// Obter usuário por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResultadoOperacao<UsuarioDto>>> ObterUsuarioPorId(Guid id)
        {
            var resultado = await _usuarioService.ObterUsuarioPorIdAsync(id);
            
            if (!resultado.Sucesso)
                return NotFound(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Obter usuário por telefone
        /// </summary>
        [HttpGet("telefone/{telefone}")]
        public async Task<ActionResult<ResultadoOperacao<UsuarioDto>>> ObterUsuarioPorTelefone(string telefone)
        {
            var resultado = await _usuarioService.ObterUsuarioPorTelefoneAsync(telefone);
            
            if (!resultado.Sucesso)
                return NotFound(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Obter histórico de transações do usuário
        /// </summary>
        [HttpGet("{id:guid}/transacoes")]
        public async Task<ActionResult<ResultadoOperacao<List<TransacaoDto>>>> ObterHistoricoTransacoes(Guid id)
        {
            var resultado = await _usuarioService.ObterHistoricoTransacoesAsync(id);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Obter pedidos do usuário
        /// </summary>
        [HttpGet("{id:guid}/pedidos")]
        public async Task<ActionResult<ResultadoOperacao<List<PedidoDto>>>> ObterPedidosUsuario(Guid id)
        {
            var resultado = await _usuarioService.ObterPedidosUsuarioAsync(id);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return Ok(resultado);
        }
    }
}
