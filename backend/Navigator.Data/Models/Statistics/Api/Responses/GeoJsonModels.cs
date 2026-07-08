using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record GeoJsonFeatureCollection(
    [property: JsonPropertyName("type")]
    [property: Description("GeoJSON object type. Always FeatureCollection.")]
    string Type,

    [property: JsonPropertyName("features")]
    [property: Description("GeoJSON features contained in the collection.")]
    IReadOnlyList<GeoJsonFeature> Features
);

public sealed record GeoJsonFeature(
    [property: JsonPropertyName("type")]
    [property: Description("GeoJSON object type. Always Feature.")]
    string Type,

    [property: JsonPropertyName("id")]
    [property: Description("Stable feature identifier.")]
    string Id,

    [property: JsonPropertyName("geometry")]
    [property: Description("Feature geometry.")]
    GeoJsonPoint Geometry,

    [property: JsonPropertyName("properties")]
    [property: Description("Application-specific feature properties.")]
    IReadOnlyDictionary<string, object?> Properties
);

public sealed record GeoJsonPoint(
    [property: JsonPropertyName("type")]
    [property: Description("GeoJSON geometry type. Always Point.")]
    string Type,

    [property: JsonPropertyName("coordinates")]
    [property: Description("Point coordinates in longitude-latitude order.")]
    double[] Coordinates
);
