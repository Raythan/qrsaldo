using Microsoft.AspNetCore.Mvc;
using QRSaldo.API.DTOs;
using QRSaldo.API.Models;
using QRSaldo.API.Services;

namespace QRSaldo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarracasController : ControllerBase
    {
        private readonly IBarracaService _barracaService;
        private readonly IQRCodeService _qrCodeService;

        public BarracasController(IBarracaService barracaService, IQRCodeService qrCodeService)
        {
            _barracaService = barracaService;
            _qrCodeService = qrCodeService;
        }

        /// <summary>
        /// Obter todas as barracas ativas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultadoOperacao<List<BarracaDto>>>> ObterBarracas()
        {
            var resultado = await _barracaService.ObterBarracasAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Obter barraca por ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResultadoOperacao<BarracaDto>>> ObterBarracaPorId(int id)
        {
            var resultado = await _barracaService.ObterBarracaPorIdAsync(id);
            
            if (!resultado.Sucesso)
                return NotFound(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Obter produtos de uma barraca
        /// </summary>
        [HttpGet("{id:int}/produtos")]
        public async Task<ActionResult<ResultadoOperacao<List<ProdutoDto>>>> ObterProdutosPorBarraca(int id)
        {
            var resultado = await _barracaService.ObterProdutosPorBarracaAsync(id);
            return Ok(resultado);
        }

        /// <summary>
        /// Obter pedidos de uma barraca
        /// </summary>
        [HttpGet("{id:int}/pedidos")]
        public async Task<ActionResult<ResultadoOperacao<List<PedidoDto>>>> ObterPedidosPorBarraca(
            int id, 
            [FromQuery] string? status = null)
        {
            StatusPedido? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusPedido>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            var resultado = await _barracaService.ObterPedidosPorBarracaAsync(id, statusEnum);
            return Ok(resultado);
        }

        /// <summary>
        /// Atualizar status de um pedido
        /// </summary>
        [HttpPatch("pedidos/{pedidoId:guid}/status")]
        public async Task<ActionResult<ResultadoOperacao<PedidoDto>>> AtualizarStatusPedido(
            Guid pedidoId, 
            AtualizarStatusPedidoDto dto)
        {
            var resultado = await _barracaService.AtualizarStatusPedidoAsync(pedidoId, dto);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Gerar QR Code para compra de produto
        /// </summary>
        [HttpGet("produtos/{produtoId:int}/qrcode")]
        public ActionResult ObterQRCodeProduto(int produtoId)
        {
            try
            {
                // URL que o usuário vai escanear para comprar o produto
                var urlCompra = $"{Request.Scheme}://{Request.Host}/consumir?produtoId={produtoId}";
                var qrCodeBytes = _qrCodeService.GerarQRCode(urlCompra);
                
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultadoOperacao<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao gerar QR Code",
                    Erros = new List<string> { ex.Message }
                });
            }
        }
    }
}
