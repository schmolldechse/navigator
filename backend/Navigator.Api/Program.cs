using Navigator.Api.Converters;
using Navigator.Api.Mapping;
using Navigator.Data;
using Navigator.Data.Mapping;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        options.JsonSerializerOptions.Converters.Add(new LocalDateTimeOffsetConverter());
    });
builder.Services.AddOpenApi();

// include Navigator.Data
builder.Services.AddServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(StationProfile))
    .AddAutoMapper(typeof(TimetableProfile));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger", options =>
    {
        options.WithTitle("Navigator Backend");
        options.WithTheme(ScalarTheme.Default);
    });

    app.MapGet("/", [ExcludeFromDescription] () => Results.Redirect("/swagger"));
    app.MapGet("/swagger.json", [ExcludeFromDescription] async (HttpContext context) =>
    {
        var response = await context.RequestServices
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient()
            .GetAsync($"{context.Request.Scheme}://{context.Request.Host}/openapi/v1.json");

        var content = await response.Content.ReadAsStringAsync();
        return Results.Text(content, "application/json");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
