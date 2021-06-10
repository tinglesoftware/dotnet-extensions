using System.Collections.Generic;

namespace Tingle.Extensions.JsonPatch.Tests
{
    public class AnotherDTO
    {
        public int TaskNum { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DtoKind Kind { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public enum DtoKind
    {
        Simple,
        Complex,
        JustCrap,
    }
}
