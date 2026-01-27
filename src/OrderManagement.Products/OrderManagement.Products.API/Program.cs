using OrderManagement.Products.API.Application.Services;
using OrderManagement.Products.API.Infrastructure;
using OrderManagement.Products.API.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTransient<IProductService, ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
    var seeder = new DatabaseSeeder(context, eventPublisher);
    await seeder.SeedAsync();
}

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