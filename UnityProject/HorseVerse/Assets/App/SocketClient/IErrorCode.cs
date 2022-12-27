using System.Collections.Generic;

public interface IErrorCodeConfiguration
{
    public int SuccessCode { get; }
    public int[] HandleCode { get; }
    public IReadOnlyDictionary<int, string> ErrorCodeMessage { get; }
}