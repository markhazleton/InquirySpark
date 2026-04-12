using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DecisionSpark.Swagger;

/// <summary>
/// Schema filter to handle object and List&lt;object&gt; types in Swagger documentation.
/// </summary>
public class ObjectTypeSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema openApiSchema)
        {
            return;
        }

        // Handle List<object> properties
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = context.Type.GetGenericArguments()[0];
            if (elementType == typeof(object))
            {
                openApiSchema.Type = JsonSchemaType.Array;
                openApiSchema.Items = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    AdditionalPropertiesAllowed = true,
                    Description = "Dynamic object - can contain any JSON structure"
                };
            }
        }

        // Handle Dictionary<string, object> properties
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var valueType = context.Type.GetGenericArguments()[1];
            if (valueType == typeof(object))
            {
                openApiSchema.Type = JsonSchemaType.Object;
                openApiSchema.AdditionalPropertiesAllowed = true;
                openApiSchema.AdditionalProperties = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Dynamic value - can be any JSON type"
                };
            }
        }
    }
}
