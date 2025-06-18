using Microsoft.EntityFrameworkCore;
using QRSaldo.API.Data;
using QRSaldo.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "QRSaldo API", 
        Version = "v1",
        Description = "API para sistema de saldo via QR Code para quermesses"
    });
});

// Configurar Entity Framework com SQLite
builder.Services.AddDbContext<QRSaldoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=qrsaldo.db"));

// Registrar serviços
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ISaldoService, SaldoService>();
builder.Services.AddScoped<IBarracaService, BarracaService>();

// Configurar CORS para permitir acesso local
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QRSaldo API v1");
        c.RoutePrefix = "swagger";
    });
}

// Criar banco de dados se não existir
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<QRSaldoContext>();
    context.Database.EnsureCreated();
}

app.UseCors("LocalPolicy");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

// Páginas estáticas para as interfaces
app.MapFallbackToFile("/", "index.html");
app.MapFallbackToFile("/caixa", "caixa.html");
app.MapFallbackToFile("/usuario", "usuario.html");
app.MapFallbackToFile("/barraca/{id:int}", "barraca.html");
app.MapFallbackToFile("/creditar", "creditar.html");
app.MapFallbackToFile("/consumir", "consumir.html");

app.Run();
