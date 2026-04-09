using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite("DataSource=Data/Order.db"));
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHttpClient<ICustomerClient, CustomerClient>(client =>
{
   client.BaseAddress = new Uri("http://customerservice:8080/"); 
});

builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
   client.BaseAddress = new Uri("http://productservice:8080/"); 
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();



app.Run();


