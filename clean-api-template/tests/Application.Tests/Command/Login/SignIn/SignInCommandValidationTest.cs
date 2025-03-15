namespace Application.Tests.Command;
using FluentValidation.TestHelper;
using MsClean.Application;

public class SignInCommandValidationTest
{
    private static readonly SignInCommandValidation _validator = new();

    [Fact]
    public void Validator_ShouldNotHaveValidationErrorFor_UserName()
    {
        // Arrange
        var command = new SignInCommand
        {
            UserName = "gnavarro",
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(command => command.UserName);
    }

    [Fact]
    public void Validator_ShouldHaveValidationErrorFor_Name()
    {
        // Arrange
        var command = new SignInCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.UserName);
    }
}
