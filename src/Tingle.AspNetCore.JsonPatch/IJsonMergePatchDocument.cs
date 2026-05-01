using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace Tingle.AspNetCore.JsonPatch;

public interface IJsonMergePatchDocument
{
    JsonSerializerOptions SerializerOptions { get; set; }

    IList<Operation> GetOperations();
}
