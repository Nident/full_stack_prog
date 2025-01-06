using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO;

var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var files = Directory.GetFiles(rootPath);
Console.WriteLine("Files in wwwroot:");
foreach (var file in files)
{
    Console.WriteLine(file);
}

var builder = WebApplication.CreateBuilder(args);

// Настройка сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Простое хранилище пользователей в памяти
var users = new Dictionary<string, (string Password, string Email)>();

// Обслуживание статичных файлов
app.UseStaticFiles();

// Страница логина
app.MapGet("/", () => Results.Redirect("/login.html"));

// Страница логина с обработкой сессии
app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    // Проверка логина и пароля
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        return Results.Redirect("/login.html?error=emptyfields");
    }

    // Проверка данных пользователей
    if (users.ContainsKey(username) && users[username].Password == password)
    {
        context.Session.SetString("User", username);
        return Results.Redirect("/index.html");
    }

    return Results.Redirect("/login.html?error=invalidcredentials");
});


// Страница регистрации
// app.MapGet("/register.html", () => Results.File("wwwroot/register.html"));
app.MapGet("/register.html", () =>
{
    var path = "/Users/nident/Desktop/RSREU/Наумов/own project/MyWebApp/wwwroot/register.html";
    Console.WriteLine("Attempting to load register.html from path: /Users/nident/Desktop/RSREU/Наумов/own project/MyWebApp/wwwroot/register.html");
    return Results.File(path);
});

// Обработка регистрации
app.MapPost("/register", async (HttpContext context) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var username = form["username"];
        var password = form["password"];
        var email = form["email"];

        // Проверка на пустые поля
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            return Results.Redirect("/register.html?error=emptyfields");
        }

        // Проверка на наличие пользователя
        if (!users.ContainsKey(username))
        {
            users[username] = (password, email);
            // Перенаправление на страницу входа с параметром регистрации успешна
            return Results.Redirect("/login.html?registered=true");
        }

        return Results.Redirect("/register.html?error=userexists");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during registration: {ex.Message}");
        return Results.StatusCode(500); // Возвращаем ошибку 500
    }
});

// Запуск приложения
app.Run();