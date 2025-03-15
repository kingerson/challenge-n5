namespace MsClean.Presentation;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ApiError
{
    public ApiError(string[] message, string innerMessage, string stackTrace)
    {
        Messages = message;
        InnerMessage = innerMessage;
        StackTrace = stackTrace;
    }

    public string[] Messages { get; set; }
    public string InnerMessage { get; set; }
    public string StackTrace { get; set; }
}
