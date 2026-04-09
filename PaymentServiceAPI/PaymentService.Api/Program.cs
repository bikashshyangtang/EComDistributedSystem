using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlite("DataSource=Data/Payment.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMqPayPublisher>();
builder.Services.AddHostedService<RabbitMqCreateConsumer>();
builder.Services.AddHostedService<RabbitMqCancelConsumer>();

builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
   client.BaseAddress = new Uri("http://productservice:8080/"); 
});

builder.Services.AddHttpClient<IOrderClient, OrderClient>(client =>
{
   client.BaseAddress = new Uri("http://orderservice:8080/"); 
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    dbContext.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


