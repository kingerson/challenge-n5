namespace Application.Tests.Command;
using FluentAssertions;
using MsClean.Application;

public class ModifyPermissionCommandValidationTest
{
    private static readonly ModifyPermissionCommandValidation _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new ModifyPermissionCommand(
            Id: 1,
            EmployeeName: "Gerson",
            EmployeeLastName: "Navarro",
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    public void Validate_InvalidId_ShouldFail(int invalidId)
    {
        // Arrange
        var command = new ModifyPermissionCommand(
            Id: invalidId,
            EmployeeName: "Gerson",
            EmployeeLastName: "Navarro",
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(ModifyPermissionCommand.Id))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.IdCannotBeNullOrEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidEmployeeName_ShouldFail(string? invalidName)
    {
        // Arrange
        var command = new ModifyPermissionCommand(
            Id: 1,
            EmployeeName: invalidName,
            EmployeeLastName: "Navarro",
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(ModifyPermissionCommand.EmployeeName))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.NameCannotBeNullOrEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidEmployeeLastName_ShouldFail(string? invalidLastName)
    {
        // Arrange
        var command = new ModifyPermissionCommand(
            Id: 1,
            EmployeeName: "Gerson",
            EmployeeLastName: invalidLastName,
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(ModifyPermissionCommand.EmployeeLastName))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.LastNameCannotBeNullOrEmpty);
    }

    [Theory]
    [InlineData(0)]
    public void Validate_InvalidPermissionTypeId_ShouldFail(int invalidId)
    {
        // Arrange
        var command = new ModifyPermissionCommand(
            Id: 1,
            EmployeeName: "Gerson",
            EmployeeLastName: "Navarro",
            PermissionTypeId: invalidId
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(ModifyPermissionCommand.PermissionTypeId))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.PermissionTypeCannotBeNullOrEmpty);
    }
}
