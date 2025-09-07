using Binance.Net.Clients;
using CryptoExchange.Net.Authentication;
using CoinAPI;
using CoinAPI.Coin;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

cCommon.fnSetAllLeagues();
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
cCommon.connectionString = configuration.GetSection("ConnectionStrings:connectionString").Value??"";

// Add services to the container.
BinanceRestClient.SetDefaultOptions(options =>
        {
            options.ApiCredentials = new ApiCredentials(configuration.GetSection("AppSettings:apiKey").Value??"", configuration.GetSection("AppSettings:secretKey").Value!); 
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Başlığı",
        Description = "API Açıklaması",
        Contact = new OpenApiContact
        {
            Name = "Destek Ekibi",
            Email = "support@example.com"
        }
    });
});
builder.Services.AddHostedService<cCoinWorker>();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
