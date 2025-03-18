namespace Application.Tests.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetAllPermissionNotificationHandlerTest
{
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;

    private readonly GetAllPermissionNotificationHandler _handler;

    public GetAllPermissionNotificationHandlerTest()
    {
        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexBulkAsync(It.IsAny<IEnumerable<Permission>>(), It.IsAny<string>())).ReturnsAsync(true);

        _handler = new GetAllPermissionNotificationHandler(_kafkaServiceMock.Object, _elasticSearchServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallKafkaProduceAsync_AndElasticSearchBulkIndex()
    {
        // Arrange
        var permissions = new List<Permission> { new() { Id = 1 }, new() { Id = 2 } };
        var notification = new GetAllPermissionNotification(permissions);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "get"), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexBulkAsync(permissions, "permissions"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenKafkaFails_ShouldNotCallElasticSearch()
    {
        // Arrange
        var permissions = new List<Permission> { new() { Id = 3 } };
        var notification = new GetAllPermissionNotification(permissions);

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
                                                    .ThrowsAsync(new MsCleanException("Kafka Failure"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Kafka Failure");

        _elasticSearchServiceMock.Verify(e => e.IndexBulkAsync(It.IsAny<IEnumerable<Permission>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenElasticSearchFails_ShouldThrowException()
    {
        // Arrange
        var permissions = new List<Permission> { new() { Id = 4 } };
        var notification = new GetAllPermissionNotification(permissions);

        _elasticSearchServiceMock.Setup(e => e.IndexBulkAsync(It.IsAny<IEnumerable<Permission>>(), It.IsAny<string>()))
                                                                        .ThrowsAsync(new MsCleanException("ElasticSearch Failure"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("ElasticSearch Failure");

        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "get"), Times.Once);
    }
}
