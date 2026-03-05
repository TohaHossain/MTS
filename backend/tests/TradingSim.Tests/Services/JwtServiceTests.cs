using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;
using TradingSim.Infrastructure.Security; // adjust namespace
using Xunit;

namespace TradingSim.Tests.Services;

public class JwtServiceTests
{
    private static IConfiguration BuildTestConfig()
    {
        var dict = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "TradingSim",
            ["Jwt:Audience"] = "TradingSim",
            ["Jwt:Key"] = "THIS_IS_A_TEST_KEY_THAT_IS_LONG_ENOUGH_1234567890"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
    }

    [Fact]
    public void GenerateToken_Should_Return_ValidJwt()
    {
        // Arrange
        var cfg = BuildTestConfig();
        var svc = new JwtService(cfg);

        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            Role = UserRole.Client
        };

        // Act
        var token = svc.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();

        var jwt = handler.ReadJwtToken(token);
        jwt.Issuer.Should().Be("TradingSim");
        jwt.Audiences.Should().Contain("TradingSim");

        jwt.Claims.Should().Contain(c => c.Type == "sub" && c.Value == user.Id);
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == user.Role.ToString());
    }
}