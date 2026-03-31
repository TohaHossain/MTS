using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TradingSim.Api.Middleware;
using TradingSim.Application.UseCases.Auth;
using TradingSim.Application.UseCases.Instruments;
using TradingSim.Application.UseCases.Orders;
using TradingSim.Application.UseCases.Trades;
using TradingSim.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// ✅ Load Mongo from ENV in production (Render / Atlas)
if (!builder.Environment.IsDevelopment())
{
    var mongoConnection = Environment.GetEnvironmentVariable("MONGO_URI");

    if (!string.IsNullOrWhiteSpace(mongoConnection))
    {
        builder.Configuration["Mongo:ConnectionString"] = mongoConnection;
    }
}


// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);


// Use cases
builder.Services.AddScoped<SignupUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<CreateInstrumentUseCase>();
builder.Services.AddScoped<ListInstrumentsUseCase>();
builder.Services.AddScoped<PlaceOrderUseCase>();
builder.Services.AddScoped<CancelOrderUseCase>();
builder.Services.AddScoped<ListOpenOrdersUseCase>();
builder.Services.AddScoped<GetMyTradesUseCase>();
builder.Services.AddScoped<GetAllTradesUseCase>();


// Power Use Cases
builder.Services.AddScoped<TradingSim.Application.UseCases.Power.RequestPowerUseCase>();
builder.Services.AddScoped<TradingSim.Application.UseCases.Power.ListMyPowerRequestsUseCase>();
builder.Services.AddScoped<TradingSim.Application.UseCases.Power.ListPendingPowerRequestsUseCase>();
builder.Services.AddScoped<TradingSim.Application.UseCases.Power.ReviewPowerUseCase>();
builder.Services.AddScoped<TradingSim.Application.UseCases.Power.GetBalanceUseCase>();


// JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"]!;
var issuer = builder.Configuration["Jwt:Issuer"]!;
var audience = builder.Configuration["Jwt:Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Render uses HTTPS already

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();


// ✅ FIXED CORS (DEV + PROD)
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173", // dev
                "https://mts-frontend-latest.onrender.com" // production
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
            // ❌ DO NOT use AllowCredentials() unless needed
    });
});


var app = builder.Build();


// Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();


// ✅ IMPORTANT ORDER
app.UseCors("cors");

// ❌ Do NOT force HTTPS on Render (it already handles HTTPS)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();