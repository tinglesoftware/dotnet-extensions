using System.Collections.Generic;

namespace Tingle.Extensions.DataAnnotations.Tests.Models
{
    public class ClassWithDictionary
    {
        public List<Dictionary<string, Child>>? Objects { get; set; }
    }
}
