using System.Diagnostics.CodeAnalysis;

namespace NMediator.Core.Result
{
    /// <summary>
    /// A Functional Error for request result
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set;}
        /// <summary>
        /// Error description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Error details
        /// </summary>
        public object Details { get; set; }
    }
}
