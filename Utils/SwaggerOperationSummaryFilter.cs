using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace FoodDeliveryBackend.Utils
{
    public class SwaggerOperationSummaryFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var summaryAttribute = context.MethodInfo.GetCustomAttribute<SwaggerOperationSummaryAttribute>();
            if (summaryAttribute != null)
            {
                operation.Summary = summaryAttribute.Summary;
            }
        }
    }
}
