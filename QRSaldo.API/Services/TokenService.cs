using System.Security.Cryptography;
using System.Text;

namespace QRSaldo.API.Services
{
    public interface ITokenService
    {
        string GerarToken(decimal valor, DateTime expiracao);
        bool ValidarToken(string token, decimal valor, out DateTime expiracao);
    }

    public class TokenService : ITokenService
    {
        private readonly string _chaveSecreta;

        public TokenService(IConfiguration configuration)
        {
            _chaveSecreta = configuration.GetValue<string>("TokenService:ChaveSecreta") 
                ?? "ChaveSecretaPadrao_AltereEstaChave_ParaProducao_2024!@#";
        }

        public string GerarToken(decimal valor, DateTime expiracao)
        {
            var dados = $"{valor:F2}|{expiracao:yyyy-MM-dd HH:mm:ss}";
            var hash = GerarHash(dados);
            var tokenCompleto = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(dados))}:{hash}";
            return tokenCompleto;
        }

        public bool ValidarToken(string token, decimal valor, out DateTime expiracao)
        {
            expiracao = DateTime.MinValue;
            
            try
            {
                var partes = token.Split(':');
                if (partes.Length != 2) return false;

                var dadosBase64 = partes[0];
                var hashRecebido = partes[1];

                var dados = Encoding.UTF8.GetString(Convert.FromBase64String(dadosBase64));
                var hashCalculado = GerarHash(dados);

                if (hashCalculado != hashRecebido) return false;

                var partesToken = dados.Split('|');
                if (partesToken.Length != 2) return false;

                if (!decimal.TryParse(partesToken[0], out var valorToken)) return false;
                if (!DateTime.TryParse(partesToken[1], out expiracao)) return false;

                return valorToken == valor && expiracao > DateTime.Now;
            }
            catch
            {
                return false;
            }
        }

        private string GerarHash(string dados)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_chaveSecreta));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dados));
            return Convert.ToBase64String(hash);
        }
    }
}
