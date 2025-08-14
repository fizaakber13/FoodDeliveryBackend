using System;

namespace FoodDeliveryBackend.Utils
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class SwaggerOperationSummaryAttribute : Attribute
    {
        public string Summary { get; }

        public SwaggerOperationSummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
