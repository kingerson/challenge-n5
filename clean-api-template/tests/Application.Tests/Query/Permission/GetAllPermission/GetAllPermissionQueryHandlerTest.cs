namespace Application.Tests.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetAllPermissionQueryHandlerTest
{
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly GetAllPermissionQueryHandler _handler;

    public GetAllPermissionQueryHandlerTest()
    {
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();
        _mapperMock = new Mock<IMapper>();

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexBulkAsync(It.IsAny<IEnumerable<Permission>>(),It.IsAny<string>())).ReturnsAsync(true);

        _handler = new GetAllPermissionQueryHandler(_permissionRepositoryMock.Object,_kafkaServiceMock.Object,_elasticSearchServiceMock.Object,_mapperMock.Object);
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
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "get"), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<Permission>>(permissionViewModels), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexBulkAsync(It.IsAny<IEnumerable<Permission>>(), It.IsAny<string>()), Times.Once);


        result.Should().BeEquivalentTo(permissionViewModels);
    }

    [Fact]
    public void Constructor_ShouldThrow_IfRepositoryIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            null!,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object,
            _mapperMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("permissionQueryRepository");
    }

    [Fact]
    public void Constructor_ShouldThrow_IfKafkaServiceIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            null!,
            _elasticSearchServiceMock.Object,
            _mapperMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("kakfaService");
    }

    [Fact]
    public void Constructor_ShouldThrow_IfElasticSearchServiceIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            _kafkaServiceMock.Object,
            null!,
            _mapperMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("elasticSearchService");
    }

    [Fact]
    public void Constructor_ShouldThrow_IfMapperIsNull()
    {
        // Act
        Action act = () => new GetAllPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("mapper");
    }

}
