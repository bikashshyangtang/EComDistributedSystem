
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using System.Net.NetworkInformation;
using ApiGateway.Gateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway API", Version = "v1" });
});
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
   client.BaseAddress = new Uri("http://productservice:8080/"); 
});

builder.Services.AddHttpClient<IOrderClient, OrderClient>(client =>
{
   client.BaseAddress = new Uri("http://orderservice:8080/"); 
});

builder.Services.AddHttpClient<ICustomerClient, CustomerClient>(client =>
{
   client.BaseAddress = new Uri("http://customerservice:8080/"); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

//app.UseHttpsRedirection();

app.UseSwagger();

// app.UseSwaggerForOcelotUI(options =>
// {
//     options.PathToSwaggerGenerator = "/swagger/docs";
// });

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/gateway/swagger.json", "Gateway Controllers");
    c.RoutePrefix = "swagger-ui";
});

// 2. Ocelot proxied routes — served at /api-docs/index.html
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
    //options.RoutePrefix = "api-docs";
});


app.MapControllers();
await app.UseOcelot();

app.Run();
