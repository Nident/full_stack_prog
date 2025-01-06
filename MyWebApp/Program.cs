using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Включение сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseSession();  // Подключение сессий
app.UseStaticFiles();  // Обслуживание статических файлов (например, HTML и CSS)

app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    // Пример простой проверки логина и пароля
    if (username == "admin" && password == "123")
    {
        context.Session.SetString("User", username);  // Установка сессии
        return Results.Redirect("/index.html");  // Переход на главную
    }
    return Results.Redirect("/login.html?error=true");  // Возврат на логин с ошибкой
});

// Маршрут по умолчанию (начальная страница)
app.MapGet("/", () => Results.Redirect("/login.html"));

app.Run();