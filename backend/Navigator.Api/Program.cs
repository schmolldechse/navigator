using Navigator.Api.Converters;
using Navigator.Api.Mapping;
using Navigator.Api.OpenApi;
using Navigator.Data;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Exceptions;
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
builder.Services.AddOpenApi(options => options.AddSchemaTransformer<StringEnumSchemaTransformer>());

// logging
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("app", "Navigator.Preflight")
    .Enrich.WithProperty("env", builder.Environment.EnvironmentName));

// include Navigator.Data
builder.Services.AddServices(builder.Configuration);

builder.Services.AddSingleton<JourneyMapper>()
    .AddSingleton<StationMapper>()
    .AddSingleton<StatisticsMapper>()
    .AddSingleton<TimetableMapper>();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference("/swagger", options =>
{
    options.WithTitle("Navigator Backend");
    options.WithTheme(ScalarTheme.Default);
});

app.MapGet("/", [ExcludeFromDescription] () => Results.Redirect("/swagger"));
app.MapGet("/swagger.json", [ExcludeFromDescription] async (HttpContext context) =>
{
    var client = context.RequestServices
        .GetRequiredService<IHttpClientFactory>()
        .CreateClient();

    var response = await client
        .GetAsync($"{context.Request.Scheme}://{context.Request.Host}/openapi/v1.json",
        HttpCompletionOption.ResponseHeadersRead);

    response.EnsureSuccessStatusCode();

    return Results.File(
        await response.Content.ReadAsStreamAsync(),
        "application/json",
        "swagger.json");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
