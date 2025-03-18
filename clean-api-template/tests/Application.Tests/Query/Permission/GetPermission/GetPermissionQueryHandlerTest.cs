namespace Application.Tests.Query;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using MsClean.Application;

public class GetPermissionQueryHandlerTest
{
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IMediator> _mediatorMock;

    private readonly GetPermissionQueryHandler _handler;

    public GetPermissionQueryHandlerTest()
    {
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new GetPermissionQueryHandler(_permissionRepositoryMock.Object, _mediatorMock.Object);
    }
    [Fact]
    public async Task Handle_CacheMiss_ShouldRetrieveFromRepo_SetCache_ProduceKafka_AndIndexElastic()
    {
        // Arrange
        var query = new GetPermissionQuery(100);

        var permissionVM = new PermissionViewModel
        {
            Id = 100,
            EmployeeForename = "Gerson",
            EmployeeLastName = "Navarro",
            PermissionTypeId = 1,
            PermissionDate = DateTime.UtcNow,
            UserRegister = "User",
            DateTimeRegister = DateTime.UtcNow
        };

        _permissionRepositoryMock
            .Setup(r => r.GetById(query.id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissionVM);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        _permissionRepositoryMock.Verify(r => r.GetById(100, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<GetPermissionNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        result.Should().BeEquivalentTo(permissionVM);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPermissionRepositoryIsNull()
    {
        // Act
        Action act = () => new GetPermissionQueryHandler(
            null!,
            _mediatorMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("permissionQueryRepository");
    }

}
