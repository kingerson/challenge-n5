namespace Application.Tests.Command;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using MsClean.Application;

public class SignInCommandHandlerTest
{
    private readonly SignInCommandHandler _handler;
    private readonly Mock<IConfiguration> _configurationMock;
    public SignInCommandHandlerTest()
    {
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["JwtSecurityToken:Key"]).Returns("super-secret-key-value!G3rs0nN@v@rr0");
        _configurationMock.Setup(c => c["JwtSecurityToken:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["JwtSecurityToken:Audience"]).Returns("TestAudience");

        _handler = new SignInCommandHandler(_configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldGenerateValidJwtToken()
    {
        // Arrange
        var command = new SignInCommand
        {
            UserName = "gnavarro"
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var token = await _handler.Handle(command, cancellationToken);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Should().NotBeNull();
        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");

        jwtToken.Claims.Should().ContainSingle(c => c.Type == "rol" && c.Value == "admin");
        jwtToken.Claims.Should().ContainSingle(c => c.Type == "username" && c.Value == "gnavarro");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        IConfiguration? configuration = null;

        // Act
        Action act = () => new SignInCommandHandler(configuration);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*configuration*");
    }
}
