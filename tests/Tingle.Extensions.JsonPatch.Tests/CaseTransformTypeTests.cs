using System.Text.Json;
using Tingle.Extensions.JsonPatch.Helpers;
using Xunit;

namespace Tingle.Extensions.JsonPatch.Tests
{
    public class CaseTransformTypeTests
    {
        [Fact]
        public void CaseTransformType_UpperCase_SerializeCorrectly()
        {
            var patchDoc = new JsonPatchDocument<SimpleDTO>(CaseTransformType.UpperCase);
            patchDoc.Add(o => o.StringProperty, "B");

            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            var result = JsonSerializer.Serialize(patchDoc, options);

            Assert.Equal("[{\"value\":\"B\",\"path\":\"/STRINGPROPERTY\",\"op\":\"add\"}]", result);
        }

        [Fact]
        public void CaseTransformType_CamelCase_SerializeCorrectly()
        {
            var patchDoc = new JsonPatchDocument<SimpleDTO>(CaseTransformType.CamelCase);
            patchDoc.Add(o => o.StringProperty, "B");

            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            var result = JsonSerializer.Serialize(patchDoc, options);

            Assert.Equal("[{\"value\":\"B\",\"path\":\"/stringProperty\",\"op\":\"add\"}]", result);
        }

        [Fact]
        public void CaseTransformType_Original_SerializeCorrectly()
        {
            var patchDoc = new JsonPatchDocument<SimpleDTO>(CaseTransformType.OriginalCase);
            patchDoc.Add(o => o.StringProperty, "B");

            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            var result = JsonSerializer.Serialize(patchDoc, options);

            Assert.Equal("[{\"value\":\"B\",\"path\":\"/StringProperty\",\"op\":\"add\"}]", result);
        }

        [Fact]
        public void CaseTransformType_LowerCase_IsDefaultAndSerializeCorrectly()
        {
            var patchDoc = new JsonPatchDocument<SimpleDTO>();
            patchDoc.Add(o => o.StringProperty, "B");

            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            var result = JsonSerializer.Serialize(patchDoc, options);

            Assert.Equal("[{\"value\":\"B\",\"path\":\"/stringproperty\",\"op\":\"add\"}]", result);
        }
    }
}
