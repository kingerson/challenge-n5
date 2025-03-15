namespace MsClean.Domain;
using System;

public class MsCleanException : Exception
{
    public MsCleanException()
    { }

    public MsCleanException(string message)
        : base(message)
    { }

    public MsCleanException(string message, Exception innerException)
        : base(message, innerException)
    { }

}
