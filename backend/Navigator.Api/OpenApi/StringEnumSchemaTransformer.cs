using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Navigator.Api.OpenApi;

internal sealed class StringEnumSchemaTransformer : IOpenApiSchemaTransformer
{
    private readonly JsonNamingPolicy _enumPolicy = JsonNamingPolicy.SnakeCaseUpper;

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;
        var actualType = Nullable.GetUnderlyingType(type) ?? type;

        if (actualType.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = null;

            schema.Enum = Enum.GetNames(actualType)
                .Select(name => (JsonNode)JsonValue.Create(_enumPolicy.ConvertName(name))!)
                .ToList();
        }

        return Task.CompletedTask;
    }
}