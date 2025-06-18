using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace QRSaldo.API.Services
{
    public interface IQRCodeService
    {
        byte[] GerarQRCode(string texto);
        string GerarQRCodeBase64(string texto);
    }

    public class QRCodeService : IQRCodeService
    {
        public byte[] GerarQRCode(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(20);
        }

        public string GerarQRCodeBase64(string texto)
        {
            var qrCodeBytes = GerarQRCode(texto);
            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
