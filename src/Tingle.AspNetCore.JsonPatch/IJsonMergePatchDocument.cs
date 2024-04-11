using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

public interface IJsonMergePatchDocument
{
    JsonSerializerOptions SerializerOptions { get; set; }

    IList<Operation> GetOperations();
}
