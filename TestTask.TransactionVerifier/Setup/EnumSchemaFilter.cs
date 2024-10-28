using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TestTask.TransactionVerifier.WebApi.Setup;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var enumNames = Enum.GetNames(context.Type).ToList();
            schema.Enum = enumNames
                .Select(name => new OpenApiString(name))
                .Cast<IOpenApiAny>()
                .ToList();
            schema.Type = "string";
            schema.Format = null;
        }
    }
}
