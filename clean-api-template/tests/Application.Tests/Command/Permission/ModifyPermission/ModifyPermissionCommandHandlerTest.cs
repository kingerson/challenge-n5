namespace Application.Tests.Command;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MsClean.Application;
using MsClean.Domain;
using MsClean.Infrastructure;

public class ModifyPermissionCommandHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Permission>> _permissionRepositoryMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly Mock<IExecutionStrategyWrapper> _executionStrategyWrapperMock;
    private readonly Mock<IKakfaService> _kafkaServiceMock;
    private readonly Mock<IElasticSearchService<Permission>> _elasticSearchServiceMock;

    private readonly ModifyPermissionCommandHandler _handler;

    public ModifyPermissionCommandHandlerTest()
    {
        _executionStrategyWrapperMock = new Mock<IExecutionStrategyWrapper>();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _permissionRepositoryMock = new Mock<IRepository<Permission>>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _kafkaServiceMock = new Mock<IKakfaService>();
        _elasticSearchServiceMock = new Mock<IElasticSearchService<Permission>>();

        _permissionRepositoryMock.Setup(r => r.Update(It.IsAny<Permission>())).Callback<Permission>(p => p.Id = 1);

        _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _executionStrategyWrapperMock.Setup(x => x.ExecuteAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(func => func());

        _unitOfWorkMock.Setup(u => u.Repository<Permission>()).Returns(_permissionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

        _kafkaServiceMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _elasticSearchServiceMock.Setup(e => e.IndexAsync(It.IsAny<Permission>())).ReturnsAsync(true);

        _handler = new ModifyPermissionCommandHandler(_unitOfWorkMock.Object, _executionStrategyWrapperMock.Object, _kafkaServiceMock.Object, _elasticSearchServiceMock.Object);
    }

    [Fact]
    public async Task Handle_PermissionExists_ShouldModify_Save_Commit_Produce_AndIndex()
    {
        // Arrange
        var command = new ModifyPermissionCommand(1, "NewName", "NewLastName", 999);

        // Simular que el permiso existe
        var existingPermission = new Permission { Id = 1 };
        _permissionRepositoryMock
            .Setup(r => r.GetById(1))
            .ReturnsAsync(existingPermission);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _permissionRepositoryMock.Verify(r => r.GetById(1), Times.Once);

        existingPermission.EmployeeForename.Should().Be("NewName");
        existingPermission.EmployeeLastName.Should().Be("NewLastName");
        existingPermission.PermissionTypeId.Should().Be(999);

        _permissionRepositoryMock.Verify(r => r.Update(existingPermission), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _kafkaServiceMock.Verify(k => k.ProduceAsync("test-topic", "modify"), Times.Once);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(existingPermission), Times.Once);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_PermissionNotFound_ShouldThrowMsCleanException()
    {
        // Arrange
        var command = new ModifyPermissionCommand(99, "Fail", "Fail", 123);

        _permissionRepositoryMock
            .Setup(r => r.GetById(99))
            .ReturnsAsync((Permission)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MsCleanException>()
            .WithMessage("Permission with id : 99 not found");

        _permissionRepositoryMock.Verify(r => r.Update(It.IsAny<Permission>()), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _kafkaServiceMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Never);
    }
    [Fact]
    public async Task Handle_WhenSaveThrowsException_ShouldRollback_AndThrowMsCleanException()
    {
        // Arrange
        var command = new ModifyPermissionCommand(2, "Error", "Case", 111);
        var existingPermission = new Permission { Id = 2 };

        _permissionRepositoryMock
            .Setup(r => r.GetById(2))
            .ReturnsAsync(existingPermission);

        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MsCleanException("DB Error"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MsCleanException>()
            .WithMessage("Error: DB Error");

        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        _kafkaServiceMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _elasticSearchServiceMock.Verify(e => e.IndexAsync(It.IsAny<Permission>()), Times.Never);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
    {
        // Act
        Action act = () => new ModifyPermissionCommandHandler(
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
        Action act = () => new ModifyPermissionCommandHandler(
            _unitOfWorkMock.Object,
            null!,
            _kafkaServiceMock.Object,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("executionStrategyWrapper");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenKafkaServiceIsNull()
    {
        // Act
        Action act = () => new ModifyPermissionCommandHandler(
            _unitOfWorkMock.Object,
            _executionStrategyWrapperMock.Object,
            null!,
            _elasticSearchServiceMock.Object
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("kakfaService");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenElasticSearchServiceIsNull()
    {
        // Act
        Action act = () => new ModifyPermissionCommandHandler(
            _unitOfWorkMock.Object,
            _executionStrategyWrapperMock.Object,
            _kafkaServiceMock.Object,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("elasticSearchService");
    }
}
