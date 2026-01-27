using Microsoft.EntityFrameworkCore;
using OrderManagement.Inventory.Application.Services;
using OrderManagement.Inventory.Application.Subscribers;
using OrderManagement.Inventory.Infrastructure;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Inventory.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IInventoryService, InventoryService>();

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

