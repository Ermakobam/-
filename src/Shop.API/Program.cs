using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shop.BLL.DTOs;
using Shop.BLL.Interfaces;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;
using Shop.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

var productRepo = app.Services.GetRequiredService<IProductRepository>();
var userRepo = app.Services.GetRequiredService<IUserRepository>();

if (userRepo.GetByEmail("customer@shop.ru") == null)
{
    userRepo.Add(new User
    {
        Id = 1,
        Email = "customer@shop.ru",
        FirstName = "Иван",
        LastName = "Иванов",
        Phone = "+7 (999) 000-11-22"
    });
}

if (productRepo.GetAll().Count == 0)
{
    var catLaptops = new Category { Id = 1, Name = "Ноутбуки" };
    var catPhones = new Category { Id = 2, Name = "Смартфоны" };
    var catAccessories = new Category { Id = 3, Name = "Аксессуары" };

    productRepo.Add(new Product { Id = 1, CategoryId = 1, Category = catLaptops, Name = "Ноутбук ASUS Vivobook 15", Sku = "ASUS-VB-15", Price = 65000, StockQuantity = 6, IsActive = true });
    productRepo.Add(new Product { Id = 2, CategoryId = 1, Category = catLaptops, Name = "Ноутбук Lenovo IdeaPad 3", Sku = "LNV-IP-3", Price = 52000, StockQuantity = 8, IsActive = true });
    productRepo.Add(new Product { Id = 3, CategoryId = 2, Category = catPhones, Name = "Смартфон Samsung Galaxy A54", Sku = "SM-A54-128", Price = 33000, StockQuantity = 12, IsActive = true });
    productRepo.Add(new Product { Id = 4, CategoryId = 2, Category = catPhones, Name = "Смартфон Xiaomi Redmi Note 13", Sku = "XMI-RN-13", Price = 24000, StockQuantity = 15, IsActive = true });
    productRepo.Add(new Product { Id = 5, CategoryId = 3, Category = catAccessories, Name = "Беспроводная мышь Logitech M185", Sku = "LOG-M185", Price = 1800, StockQuantity = 25, IsActive = true });
    productRepo.Add(new Product { Id = 6, CategoryId = 3, Category = catAccessories, Name = "Наушники JBL Tune 510BT", Sku = "JBL-T510", Price = 4500, StockQuantity = 10, IsActive = true });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/products", (IProductService productService) =>
{
    var products = productService.GetAllAvailableProducts();
    return Results.Ok(products);
});

app.MapPost("/api/users", (UserRegisterRequest request, IUserService userService) =>
{
    try
    {
        var user = userService.RegisterUser(request);
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
        var order = orderService.CreateOrder(request);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/orders/user/{userId:int}", (int userId, IOrderService orderService) =>
{
    var orders = orderService.GetOrdersByUserId(userId);
    return Results.Ok(orders);
});

app.Run();
