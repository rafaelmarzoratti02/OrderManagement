using Microsoft.EntityFrameworkCore;
using OrderManagement.Orders.Application.Services;
using OrderManagement.Orders.Application.Subscribers;
using OrderManagement.Orders.Infrastructure.Messaging;
using OrderManagement.Orders.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()!;
builder.Services.AddSingleton(rabbitMqSettings);

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

builder.Services.AddHostedService<OrderValidationResultSubscriber>();

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
