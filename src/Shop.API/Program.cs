using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shop.API.Infrastructure;
using Shop.BLL.DTOs;
using Shop.BLL.Interfaces;
using Shop.BLL.Security;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;
using Shop.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPasswordHasher, Base64PasswordHasher>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IProductRepository productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    IUserRepository userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    ShopDataSeeder.EnsureDemoData(userRepo, productRepo);
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/user", (UserRestDto req, IUserRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.PassHash))
    {
        return Results.BadRequest(new { error = "Поля Login и PassHash обязательны для заполнения" });
    }

    User? existing = repo.GetByEmail(req.Login);
    if (existing != null)
    {
        return Results.Conflict(new { error = "Пользователь с таким логином уже существует" });
    }

    User user = new User
    {
        Email = req.Login,
        PasswordHash = PasswordHashes.Sha256(req.PassHash),
        CreatedAt = DateTime.Now
    };

    repo.Add(user);
    return Results.Created($"/user/{user.Id}", new { user.Id, Login = user.Email, user.PasswordHash });
});

app.MapGet("/user/{id:int}", (int id, IUserRepository repo) =>
{
    User? user = repo.GetById(id);
    if (user == null)
    {
        return Results.NotFound(new { error = "Пользователь не найден" });
    }

    return Results.Ok(new { user.Id, Login = user.Email, PassHash = user.PasswordHash });
});

app.MapPut("/user/{id:int}", (int id, UserRestDto req, IUserRepository repo) =>
{
    User? user = repo.GetById(id);
    if (user == null)
    {
        return Results.NotFound(new { error = "Пользователь не найден" });
    }

    if (!string.IsNullOrWhiteSpace(req.Login))
    {
        User? existing = repo.GetByEmail(req.Login);
        if (existing != null && existing.Id != id)
        {
            return Results.Conflict(new { error = "Логин уже занят другим пользователем" });
        }
        user.Email = req.Login;
    }

    if (!string.IsNullOrWhiteSpace(req.PassHash))
    {
        user.PasswordHash = PasswordHashes.Sha256(req.PassHash);
    }

    repo.Update(user);
    return Results.Ok(new { message = "Данные пользователя успешно обновлены", user.Id, Login = user.Email });
});

app.MapDelete("/user/{id:int}", (int id, IUserRepository repo) =>
{
    User? user = repo.GetById(id);
    if (user == null)
    {
        return Results.NotFound(new { error = "Пользователь не найден" });
    }

    repo.Delete(id);
    return Results.Ok(new { message = $"Пользователь с Id={id} успешно удален" });
});

app.MapGet("/api/products", (IProductService productService) =>
{
    return Results.Ok(productService.GetAllAvailableProducts());
});

app.MapPost("/api/users", (UserRegisterRequest request, IUserService userService) =>
{
    try
    {
        User user = userService.RegisterUser(request);
        return Results.Created($"/api/users/{user.Id}", user);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/orders", (OrderCreateRequest request, IOrderService orderService) =>
{
    try
    {
        Order order = orderService.CreateOrder(request);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/orders/user/{userId:int}", (int userId, IOrderService orderService) =>
{
    return Results.Ok(orderService.GetOrdersByUserId(userId));
});

app.MapPost("/api/orders/{orderId:int}/cancel", (int orderId, IOrderService orderService) =>
{
    bool cancelled = orderService.CancelOrder(orderId);
    if (!cancelled)
    {
        return Results.BadRequest(new { error = "Заказ нельзя отменить" });
    }

    return Results.Ok(new { message = "Заказ отменён", orderId });
});

app.Run();
