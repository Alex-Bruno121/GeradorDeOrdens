using Microsoft.OpenApi.Models;
using OrderAccumulator.Api.Repositories.Interfaces;
using OrderAccumulator.Api.Repositories;
using OrderAccumulator.Api.Services.Interfaces;
using OrderAccumulator.Api.Services;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using OrderAccumulator.Api.Scripts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Order Accumulator API",
            Version = "v1",
            Description = "API para processamento de ordens financeiras",
            Contact = new OpenApiContact
            {
                Name = "Alexsandro Bruno",
                Email = "a.brunosousa01@gmail.com",
                Url = new Uri("https://github.com/Alex-Bruno121/")
            },
            License = new OpenApiLicense
            {
                Name = "Free License",
                Url = new Uri("https://github.com/Alex-Bruno121/")
            }
        });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

builder.Services.AddScoped<IOrderAccumulatorService, OrderAccumulatorService>();
builder.Services.AddScoped<IOrderAccumulatorRepository, OrderAccumulatorRepository>();
builder.Services.AddScoped<OrderScript>();
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("ConnectionDBOrder")));


// Adicione o CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Accumulator API");
    });
}

// Use o middleware CORS
app.UseCors("default");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();