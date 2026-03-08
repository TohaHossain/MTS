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

// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(opts => 
    {
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (Mongo, repos, jwt, engine, seed)
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
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// CORS for React dev server
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();