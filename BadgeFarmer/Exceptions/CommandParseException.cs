using System;

namespace BadgeFarmer.Exceptions;

public class CommandParseException : Exception
{
    public CommandParseException(string message, Exception exception = null) : base(message, exception)
    {
    }
}