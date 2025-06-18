# Script para publicação da release do QRSaldo
# Autor: QRSaldo Team
# Data: 18/06/2025
# Versão: 1.0.0

$versionNumber = "1.0.0"
$releaseDir = ".\Release-v$versionNumber"
$zipFileName = "QRSaldo-v$versionNumber.zip"
$readmeFile = ".\README.md"
$licenseFile = ".\LICENSE"

# Cores para mensagens
$colorSuccess = "Green"
$colorInfo = "Cyan"
$colorWarning = "Yellow"
$colorError = "Red"

# Função para exibir mensagens coloridas
function Write-ColoredMessage {
    param (
        [string]$message,
        [string]$color
    )
    Write-Host $message -ForegroundColor $color
}

# Verificar se o diretório já existe
if (Test-Path $releaseDir) {
    Write-ColoredMessage "Removendo diretório de release existente..." $colorInfo
    Remove-Item -Path $releaseDir -Recurse -Force
}

# Criar diretório para a release
Write-ColoredMessage "Criando diretório para a release v$versionNumber..." $colorInfo
New-Item -Path $releaseDir -ItemType Directory | Out-Null

# Publicar a API
Write-ColoredMessage "Publicando a API QRSaldo..." $colorInfo
dotnet publish .\QRSaldo.API\QRSaldo.API.csproj -c Release -o $releaseDir\QRSaldo -p:Version=$versionNumber

# Verificar se a publicação foi bem-sucedida
if ($LASTEXITCODE -ne 0) {
    Write-ColoredMessage "Erro ao publicar a API!" $colorError
    exit 1
}

# Copiar arquivos adicionais
Write-ColoredMessage "Copiando documentação..." $colorInfo
Copy-Item -Path $readmeFile -Destination "$releaseDir\README.md" -ErrorAction SilentlyContinue
if (Test-Path $licenseFile) {
    Copy-Item -Path $licenseFile -Destination "$releaseDir\LICENSE" -ErrorAction SilentlyContinue
}

# Criar arquivo de instruções rápidas
$quickStartContent = @"
# QRSaldo - Guia Rápido de Inicialização

## Como executar:

1. Certifique-se de ter o .NET 8.0 SDK instalado
2. Abra um terminal na pasta QRSaldo
3. Execute o comando: dotnet QRSaldo.API.dll
4. Acesse: http://localhost:5041 no navegador

## Páginas disponíveis:

- Página inicial: http://localhost:5041/
- Página do caixa: http://localhost:5041/caixa.html
- Página do usuário: http://localhost:5041/usuario.html
- Página da barraca: http://localhost:5041/barraca.html
- Documentação API: http://localhost:5041/swagger

## Configuração:

- Edite o arquivo appsettings.json para personalizar configurações

Para mais informações, consulte o README.md completo.
"@

$quickStartContent | Out-File -FilePath "$releaseDir\QUICK_START.txt" -Encoding UTF8

# Criar arquivo de lançamento
$releaseNotesContent = @"
# QRSaldo v$versionNumber - Notas de Lançamento

Data: $(Get-Date -Format "dd/MM/yyyy")

## O que há de novo

- Primeira versão oficial do QRSaldo
- Sistema completo para gestão de saldo via QR Code
- API REST com Swagger UI
- Interfaces web para caixa, usuário e barracas
- Suporte completo para funcionamento offline em rede local

## Componentes

- ASP.NET Core Web API (.NET 8.0)
- Banco de dados SQLite (gerado automaticamente)
- QR Code para crédito e consumo
- Tokens seguros HMAC SHA256
- Interface responsiva com Bootstrap

## Como atualizar

Esta é a primeira versão, não há necessidade de atualização.

## Problemas conhecidos

- Nenhum problema conhecido até o momento.

"@

$releaseNotesContent | Out-File -FilePath "$releaseDir\RELEASE_NOTES.txt" -Encoding UTF8

# Criar arquivo ZIP
Write-ColoredMessage "Criando arquivo ZIP da release..." $colorInfo
Compress-Archive -Path "$releaseDir\*" -DestinationPath $zipFileName -Force

# Mensagem final
Write-ColoredMessage "`nRelease v$versionNumber criada com sucesso!" $colorSuccess
Write-ColoredMessage "Arquivo ZIP: $zipFileName" $colorSuccess
Write-ColoredMessage "Diretório: $releaseDir" $colorSuccess
Write-ColoredMessage "`nTeste a aplicação executando: dotnet $releaseDir\QRSaldo\QRSaldo.API.dll" $colorInfo
