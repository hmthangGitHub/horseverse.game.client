using System;
using System.Runtime.Serialization;

[Serializable]
internal class UnKnownServerMessageTypeException : Exception
{
    public UnKnownServerMessageTypeException()
    {
    }

    public UnKnownServerMessageTypeException(string messageType) : base("Unknow message type :" + messageType)
    {
    }

    public UnKnownServerMessageTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UnKnownServerMessageTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}