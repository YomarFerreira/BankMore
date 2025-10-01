using Microsoft.EntityFrameworkCore;
using BankMore.Tarifa.Infrastructure.Data;
using BankMore.Tarifa.Domain.Repositories;
using BankMore.Tarifa.Infrastructure.Repositories;
using BankMore.Tarifa.Domain.Services;
using BankMore.Tarifa.Infrastructure.Services;
using BankMore.Tarifa.Application.Handlers;
using BankMore.Tarifa.Infrastructure.Kafka;
using BankMore.Tarifa.Application.Events;
using KafkaFlow;
using BankMore.Shared.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<TarifaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITarifaRepository, TarifaRepository>();

// Services
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// Handlers
builder.Services.AddScoped<TransferenciaRealizadaHandler>();

// Kafka
builder.Services.AddKafka(kafka => kafka
    .UseConsoleLog()
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { builder.Configuration["Kafka:BootstrapServers"] })
        // .CreateTopicIfNotExists("transferencia-realizada", 1, 1) // Comentado para acelerar startup sem Kafka
        // .CreateTopicIfNotExists("tarifacao-realizada", 1, 1) // Comentado para acelerar startup sem Kafka
        .AddConsumer(consumer => consumer
            .Topic("transferencia-realizada")
            .WithGroupId("tarifa-service")
            .WithBufferSize(100)
            .WithWorkersCount(10)
            .AddMiddlewares(middlewares => middlewares
                .AddDeserializer(resolver => new SystemTextJsonDeserializer())
                .AddTypedHandlers(h => h.AddHandler<TransferenciaRealizadaConsumer>())
            )
        )
        .AddProducer<TarifacaoRealizadaProducer>(producer => producer
            .DefaultTopic("tarifacao-realizada")
            .AddMiddlewares(m => m.AddSerializer(resolver => new SystemTextJsonSerializer()))
        )
    )
);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TarifaDbContext>();
    context.Database.EnsureCreated();
}

// Start Kafka
try
{
    var kafkaBus = app.Services.CreateKafkaBus();
    await kafkaBus.StartAsync();
    Console.WriteLine("Kafka iniciado com sucesso");
}
catch (Exception ex)
{
    Console.WriteLine($"AVISO: Não foi possível conectar ao Kafka: {ex.Message}");
}

app.Run();