namespace Application.Tests.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using MsClean.Application;
using MsClean.Domain;

public class GetAllPermissionQueryHandlerTest
{
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;

    private readonly GetAllPermissionQueryHandler _handler;

    public GetAllPermissionQueryHandlerTest()
    {
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new GetAllPermissionQueryHandler(_permissionRepositoryMock.Object, _mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPermissionsFromRepo_CallKafka_AndIndexBulk()
    {
        // Arrange
        var query = new GetAllPermissionQuery();
        var cancellationToken = CancellationToken.None;

        var permissionViewModels = new List<PermissionViewModel>
        {
            new() { Id = 1, EmployeeForename="Gerson", EmployeeLastName="Navarro", PermissionTypeId=10 },
            new() { Id = 2, EmployeeForename="Eduardo", EmployeeLastName="Navarro", PermissionTypeId=20 },
        };

        _permissionRepositoryMock
            .Setup(r => r.GetAll(cancellationToken))
            .ReturnsAsync(permissionViewModels);

        _mapperMock.Setup(m => m.Map<IEnumerable<Permission>>(permissionViewModels)).Returns(It.IsAny<IEnumerable<Permission>>);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        _permissionRepositoryMock.Verify(r => r.GetAll(cancellationToken), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<Permission>>(permissionViewModels), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<GetAllPermissionNotification>(), It.IsAny<CancellationToken>()), Times.Once);

        result.Should().BeEquivalentTo(permissionViewModels);
    }

    [Fact]
    public void Constructor_ShouldThrow_IfRepositoryIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            null!,
            _mediatorMock.Object,
            _mapperMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("permissionQueryRepository");
    }

    [Fact]
    public void Constructor_ShouldThrow_IfMapperIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            _mediatorMock.Object,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("mapper");
    }

}
