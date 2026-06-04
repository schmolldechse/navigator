using Navigator.Api.Converters;
using Navigator.Api.Exceptions;
using Navigator.Api.Mapping;
using Navigator.Api.OpenApi;
using Navigator.Data;
using Navigator.Observability;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    // logging
    builder.AddNavigatorObservability("Navigator.Api");

    // controllers
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
            options.JsonSerializerOptions.Converters.Add(new LocalDateTimeOffsetConverter());
        });
    builder.Services.AddOpenApi(options => options.AddSchemaTransformer<StringEnumSchemaTransformer>());

    // include Navigator.Data bridge
    builder.Services.AddServices(builder.Configuration);

    // mapping
    builder.Services.AddSingleton<JourneyMapper>()
        .AddSingleton<StationMapper>()
        .AddSingleton<TimetableMapper>();

    // global exception handling
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseMiddleware<NavigatorCorrelation>();

    app.UseNavigatorRequestLogging();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
