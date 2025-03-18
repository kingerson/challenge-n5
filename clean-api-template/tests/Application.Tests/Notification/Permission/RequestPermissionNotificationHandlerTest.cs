namespace Application.Tests.Notification;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class RequestPermissionNotificationHandlerTest
{
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;

    private readonly RequestPermissionNotificationHandler _handler;

    public RequestPermissionNotificationHandlerTest()
    {
        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexAsync(It.IsAny<Permission>(), It.IsAny<string>())).ReturnsAsync(true);

        _handler = new RequestPermissionNotificationHandler(_kafkaServiceMock.Object, _elasticSearchServiceMock.Object);
    }
    [Fact]
    public async Task Handle_ShouldCallKafkaProduceAsync_AndElasticSearchIndex()
    {
        // Arrange
        var permission = new Permission { Id = 1 };

        var notification = new RequestPermissionNotification(permission);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "request"), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(permission, "request-permissions"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenKafkaFails_ShouldStillCallElasticSearch()
    {
        // Arrange
        var permission = new Permission { Id = 2 };
        var notification = new RequestPermissionNotification(permission);

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
                                        .ThrowsAsync(new MsCleanException("Kafka Failure"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Kafka Failure");

        _elasticSearchServiceMock.Verify(e => e.IndexAsync(permission, "request-permissions"), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenElasticSearchFails_ShouldThrowException()
    {
        // Arrange
        var permission = new Permission { Id = 3 };
        var notification = new RequestPermissionNotification(permission);

        _elasticSearchServiceMock.Setup(e => e.IndexAsync(It.IsAny<Permission>(), It.IsAny<string>()))
                                                .ThrowsAsync(new MsCleanException("ElasticSearch Failure"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("ElasticSearch Failure");

        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "request"), Times.Once);
    }
}
