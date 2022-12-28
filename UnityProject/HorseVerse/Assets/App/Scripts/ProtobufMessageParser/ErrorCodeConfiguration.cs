using System.Collections.Generic;
using System.Linq;

public class ErrorCodeConfiguration : IErrorCodeConfiguration
{
    public static ErrorCodeConfiguration Initialize(MasterErrorCodeContainer masterErrorCodeContainer)
    {
        return new ErrorCodeConfiguration
        {
            SuccessCode = masterErrorCodeContainer.MasterErrorCodeIndexer.First(x => x.Value.Sucess).Key,
            HandleCode = masterErrorCodeContainer.MasterErrorCodeIndexer.Where(x => x.Value.Handle == true)
                                                 .Select(x => x.Key)
                                                 .ToArray(),
            ErrorCodeMessage = masterErrorCodeContainer.MasterErrorCodeIndexer.ToDictionary(x => x.Key, x => x.Value.DescriptionKey)
        };
    }
    public int SuccessCode { get; private set; }
    public int[] HandleCode { get; private set; }
    public IReadOnlyDictionary<int, string> ErrorCodeMessage { get; private set; }
}