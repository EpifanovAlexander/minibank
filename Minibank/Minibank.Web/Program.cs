using Minibank.Core;
using Minibank.Core.Interfaces;
using Minibank.Data;
using Minibank.Web.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICurrencyRateService, CurrencyService>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<UserFriendlyExceptionMiddleware>();


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

