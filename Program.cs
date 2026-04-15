using MyTextApi.Interfaces;
using MyTextApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в DI контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Регистрируем наш сервис (Singleton = один на все приложение)
builder.Services.AddSingleton<ITextService, TextService>();

var app = builder.Build();

// Настраиваем middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // UI будет на /scalar
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();






// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
