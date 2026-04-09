
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddOcelot(builder.Configuration);
//builder.Services.AddSwaggerGen();

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
});

// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway Controllers");
//     c.RoutePrefix = "swagger";
// });

// // 2. Ocelot proxied routes — served at /api-docs/index.html
// app.UseSwaggerForOcelotUI(options =>
// {
//     options.PathToSwaggerGenerator = "/swagger/docs";
//     //options.RoutePrefix = "api-docs";
// });

// app.UseSwaggerForOcelotUI(options =>
// {
//     // 1. Add your local Aggregator Controllers
//     // options.ConfigObject.Urls = new List<Swashbuckle.AspNetCore.SwaggerUI.UrlDescriptor>
//     // {
//     //     new Swashbuckle.AspNetCore.SwaggerUI.UrlDescriptor
//     //     {
//     //         Url = "/swagger/v1/swagger.json",
//     //         Name = "Gateway (Local Aggregator)"
//     //     }
//     // };

//     // // 2. This tells Ocelot to append the microservices to that list
//     // options.PathToSwaggerGenerator = "/swagger/docs";
//     options.PathToSwaggerGenerator = "/swagger/docs";
    
//     // This adds your local controllers to the list managed by SwaggerForOcelot
//     options.ReRouteDataFilter = (key, routes) =>
//     {
//         return true; 
//     };
// });

// app.UseSwaggerForOcelotUI(options =>
// {
//     options.PathToSwaggerGenerator = "/swagger/docs";
    
//     // This adds your local controllers to the list managed by SwaggerForOcelot
//     options.ReRouteDataFilter = (key, routes) =>
//     {
//         return true; 
//     };
// });

// If the above doesn't show the "Gateway Controllers", 
// use this standard way to ensure the local JSON is generated:
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway Aggregator (Local)");
    c.RoutePrefix = "swagger-local"; // Access this at /swagger-local to see your aggregator
});
app.MapControllers();

await app.UseOcelot();

app.Run();
