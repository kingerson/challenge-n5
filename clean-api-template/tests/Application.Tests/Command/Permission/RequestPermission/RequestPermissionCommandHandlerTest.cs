namespace Application.Tests.Command;
using System;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class RequestPermissionCommandHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Permission>> _permissionRepositoryMock;
    private readonly Mock<IRepository<PermissionType>> _permissionTypeRepositoryMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly Mock<IExecutionStrategyWrapper> _executionStrategyWrapperMock;
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;

    private readonly RequestPermissionCommandHandler _handler;

    public RequestPermissionCommandHandlerTest()
    {
        _executionStrategyWrapperMock = new Mock<IExecutionStrategyWrapper>();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _permissionRepositoryMock = new Mock<IRepository<Permission>>();
        _permissionTypeRepositoryMock = new Mock<IRepository<PermissionType>>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();

        _permissionRepositoryMock.Setup(r => r.Add(It.IsAny<Permission>())).Callback<Permission>(p => p.Id = 1).ReturnsAsync((Permission p) => p);
        _permissionTypeRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<PermissionType, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _executionStrategyWrapperMock.Setup(x => x.ExecuteAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(func => func());

        _unitOfWorkMock.Setup(u => u.Repository<Permission>()).Returns(_permissionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PermissionType>()).Returns(_permissionTypeRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexAsync(It.IsAny<Permission>())).ReturnsAsync(true);

        _handler = new RequestPermissionCommandHandler(_unitOfWorkMock.Object, _executionStrategyWrapperMock.Object, _kafkaServiceMock.Object,_elasticSearchServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPersistAndReturnPermissionId()
    {
        // Arrange
        var command = new RequestPermissionCommand("Gerson", "Navarro", 1);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _permissionRepositoryMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        resultId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallKafkaProduceAsync_AndElasticSearchIndex()
    {
        // Arrange
        var command = new RequestPermissionCommand("Gerson", "Navarro", 1);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "request"), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Once);

        resultId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenRepositoryAddThrowsException_ShouldRollbackTransaction_AndThrowMsCleanException()
    {
        // Arrange
        var command = new RequestPermissionCommand("Error", "Case", 5);

        _permissionRepositoryMock
            .Setup(r => r.Add(It.IsAny<Permission>()))
            .ThrowsAsync(new MsCleanException("DB Insert Fail"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MsCleanException>()
            .WithMessage("Error: DB Insert Fail");

        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _kafkaServiceMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaveEntitiesThrowsException_ShouldRollbackTransaction_AndThrowMsCleanException()
    {
        // Arrange
        var command = new RequestPermissionCommand("Fail", "OnSave", 4);

        _unitOfWorkMock
            .Setup(r => r.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MsCleanException("Save Fail"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MsCleanException>()
            .WithMessage("Error: Save Fail");

        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        _kafkaServiceMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Never);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
    {
        // Act
        Action act = () => new RequestPermissionCommandHandler(
            null!,
            _executionStrategyWrapperMock.Object,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("unitOfWork");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenExecutionStrategyWrapperIsNull()
    {
        // Act
        Action act = () => new RequestPermissionCommandHandler(
            _unitOfWorkMock.Object,
            null!,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("executionStrategyWrapper");
    }

}
