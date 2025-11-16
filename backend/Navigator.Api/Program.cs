using Navigator.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddData(builder.Configuration);

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
