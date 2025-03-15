namespace Presentation.Tests.Controllers;

using System.Net;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MsClean.Application;
using MsClean.Presentation.Controllers;

public class LoginControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly LoginController _controller;

    public LoginControllerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new LoginController(_mediatorMock.Object);
    }

    [Fact]
    public async Task SignIn_ShouldReturn_Ok()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();

        var command = new SignInCommand
        {
            UserName = "gnavarro",
            Password = "password",
        };

        var cancelationToken = new CancellationToken();

        _mediatorMock.Setup(x => x.Send(command, cancelationToken)).ReturnsAsync(token);

        // Act
        var result = await _controller.SignIn(command);

        // Assert

        ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        ((ObjectResult)result).Value.Should().BeEquivalentTo(token);
    }
}
