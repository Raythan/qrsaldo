using Microsoft.AspNetCore.Mvc;
using QRSaldo.API.DTOs;
using QRSaldo.API.Services;

namespace QRSaldo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaldoController : ControllerBase
    {
        private readonly ISaldoService _saldoService;
        private readonly IQRCodeService _qrCodeService;

        public SaldoController(ISaldoService saldoService, IQRCodeService qrCodeService)
        {
            _saldoService = saldoService;
            _qrCodeService = qrCodeService;
        }

        /// <summary>
        /// Criar token de crédito (usado pelo caixa)
        /// </summary>
        [HttpPost("tokens")]
        public async Task<ActionResult<ResultadoOperacao<TokenCreditoDto>>> CriarTokenCredito(CriarTokenCreditoDto dto)
        {
            var resultado = await _saldoService.CriarTokenCreditoAsync(dto);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return CreatedAtAction(nameof(ObterQRCodeCredito), new { token = resultado.Dados!.Token }, resultado);
        }

        /// <summary>
        /// Obter QR Code para crédito
        /// </summary>
        [HttpGet("tokens/{token}/qrcode")]
        public ActionResult ObterQRCodeCredito(string token)
        {
            try
            {
                // URL que o usuário vai escanear para creditar o saldo
                var urlCredito = $"{Request.Scheme}://{Request.Host}/creditar?token={Uri.EscapeDataString(token)}";
                var qrCodeBytes = _qrCodeService.GerarQRCode(urlCredito);
                
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

        /// <summary>
        /// Creditar saldo usando token
        /// </summary>
        [HttpPost("creditar")]
        public async Task<ActionResult<ResultadoOperacao<UsuarioDto>>> CreditarSaldo(CreditarSaldoDto dto)
        {
            var resultado = await _saldoService.CreditarSaldoAsync(dto);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return Ok(resultado);
        }

        /// <summary>
        /// Consumir saldo (fazer compra)
        /// </summary>
        [HttpPost("consumir")]
        public async Task<ActionResult<ResultadoOperacao<PedidoDto>>> ConsumirSaldo(ConsumirSaldoDto dto)
        {
            var resultado = await _saldoService.ConsumirSaldoAsync(dto);
            
            if (!resultado.Sucesso)
                return BadRequest(resultado);
            
            return Ok(resultado);
        }
    }
}
