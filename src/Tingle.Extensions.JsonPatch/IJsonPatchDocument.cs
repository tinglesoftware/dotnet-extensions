using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch;

public interface IJsonPatchDocument
{
    IList<Operation> GetOperations();
}
