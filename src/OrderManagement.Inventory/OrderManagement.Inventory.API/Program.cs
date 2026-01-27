using Microsoft.EntityFrameworkCore;
using OrderManagement.Inventory.Application.Services;
using OrderManagement.Inventory.Application.Subscribers;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Inventory.Infrastructure.Persistence;
using OrderManagement.Products.API.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()!;
builder.Services.AddSingleton(rabbitMqSettings);

// Application Services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

// Register Subscribers as Hosted Services
builder.Services.AddHostedService<ProductCreatedSubscriber>();
builder.Services.AddHostedService<OrderCreatedSubscriber>();

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

