
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using System.Net.NetworkInformation;
using ApiGateway.Gateway.Api.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddOcelot(builder.Configuration);
//builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5019","http://localhost:8081") // Adjust this to match your API's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
},(SwaggerUIOptions uiOptions) =>
{
    uiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway Aggregator");
});

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // AggregateController handled here
});

await app.UseOcelot();

app.Run();
