namespace Presentation.Tests.Controllers;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MsClean.Application;
using MsClean.Presentation.Controllers;

public class PermissionControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PermissionController _controller;

    public PermissionControllerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PermissionController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldSendRequestPermissionCommand_AndReturnCreated()
    {
        // Arrange
        var command = new RequestPermissionCommand("Gerson", "Navarro", 1);
        var resultId = 123;

        _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(resultId);

        // Act
        var result = await _controller.Create(command);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, default), Times.Once);

        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult!.Value.Should().Be(resultId);
        createdResult.ActionName.Should().Be(nameof(PermissionController.Create));
    }

    [Fact]
    public async Task GetById_ShouldSendGetPermissionQuery_AndReturnOkWithResult()
    {
        // Arrange
        var requestedId = 99;
        var expectedViewModel = new PermissionViewModel
        {
            Id = requestedId,
            EmployeeForename = "Gerson",
            EmployeeLastName = "Navarro"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPermissionQuery>(), default))
                        .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _controller.GetById(requestedId);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetPermissionQuery>(q => q.id == requestedId), default),Times.Once);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult!.Value.Should().BeEquivalentTo(expectedViewModel);
    }

    [Fact]
    public async Task Get_ShouldSendGetAllPermissionQuery_AndReturnOkWithCollection()
    {
        // Arrange
        var expectedList = new List<PermissionViewModel>
            {
                new() { Id = 1, EmployeeForename = "Gerson" },
                new() { Id = 2, EmployeeForename = "Eduardo" }
            };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPermissionQuery>(), default)).ReturnsAsync(expectedList);

        // Act
        var result = await _controller.Get();

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllPermissionQuery>(), default),Times.Once);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult!.Value.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public async Task Update_ShouldSendModifyPermissionCommand_AndReturnOkWithResult()
    {
        // Arrange
        var command = new ModifyPermissionCommand(1, "Gerson", "Navarro", 9);
        var mediatorResult = true; 

        _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(mediatorResult);

        // Act
        var result = await _controller.Update(command);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, default), Times.Once);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult!.Value.Should().Be(mediatorResult);
    }
}
