namespace Application.Tests.Command;
using FluentAssertions;
using MsClean.Application;

public class RequestPermissionCommandValidationTest
{
    private static readonly RequestPermissionCommandValidation _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new RequestPermissionCommand(
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
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidEmployeeName_ShouldFail(string? invalidName)
    {
        // Arrange
        var command = new RequestPermissionCommand(
            EmployeeName: invalidName,
            EmployeeLastName: "Navarro",
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RequestPermissionCommand.EmployeeName))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.NameCannotBeNullOrEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidEmployeeLastName_ShouldFail(string? invalidLastName)
    {
        // Arrange
        var command = new RequestPermissionCommand(
            EmployeeName: "Gerson",
            EmployeeLastName: invalidLastName,
            PermissionTypeId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RequestPermissionCommand.EmployeeLastName))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.LastNameCannotBeNullOrEmpty);
    }

    [Theory]
    [InlineData(0)]
    public void Validate_InvalidPermissionTypeId_ShouldFail(int invalidId)
    {
        // Arrange
        var command = new RequestPermissionCommand(
            EmployeeName: "Gerson",
            EmployeeLastName: "Navarro",
            PermissionTypeId: invalidId
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RequestPermissionCommand.PermissionTypeId))
              .Which.ErrorMessage.Should().Be(BusinessExceptionMessages.PermissionTypeCannotBeNullOrEmpty);
    }
}
