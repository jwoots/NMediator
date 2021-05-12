using System.Diagnostics.CodeAnalysis;

namespace NMediator.Core.Result
{
    [ExcludeFromCodeCoverage]
    public class Error
    {
        public string Code { get; set;}
        public string Description { get; set; }
        public object Details { get; set; }
    }
}
