namespace Application.Tests.Query;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetPermissionQueryHandlerTest
{
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;

    private readonly GetPermissionQueryHandler _handler;

    public GetPermissionQueryHandlerTest()
    {
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexAsync(It.IsAny<Permission>())).ReturnsAsync(true);

        _handler = new GetPermissionQueryHandler(_permissionRepositoryMock.Object,_kafkaServiceMock.Object,_elasticSearchServiceMock.Object);
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
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "get-id"), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Once);

        result.Should().BeEquivalentTo(permissionVM);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPermissionRepositoryIsNull()
    {
        // Act
        Action act = () => new GetPermissionQueryHandler(
            null!,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("permissionQueryRepository");
    }


    [Fact]
    public void Constructor_ShouldThrow_WhenKafkaServiceIsNull()
    {
        // Act
        Action act = () => new GetPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            null!,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("kakfaService");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenElasticSearchServiceIsNull()
    {
        // Act
        Action act = () => new GetPermissionQueryHandler(
            _permissionRepositoryMock.Object,
            _kafkaServiceMock.Object,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("elasticSearchService");
    }

}
