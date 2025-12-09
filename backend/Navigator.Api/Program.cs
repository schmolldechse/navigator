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
    });
builder.Services.AddOpenApi();

// include Navigator.Data
builder.Services.AddServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(StationProfile));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger", options =>
    {
        options.WithTitle("Navigator Backend");
        options.WithTheme(ScalarTheme.Default);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
