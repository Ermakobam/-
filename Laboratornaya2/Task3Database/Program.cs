using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new ConcurrentDictionary<int, UserEntity>();
int idCounter = 0;

string HashPassword(string raw)
{
    using var sha256 = SHA256.Create();
    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
    var sb = new StringBuilder();
    foreach (byte b in bytes)
    {
        sb.Append(b.ToString("x2"));
    }
    return sb.ToString();
}

int newId1 = System.Threading.Interlocked.Increment(ref idCounter);
users[newId1] = new UserEntity
{
    Id = newId1,
    Login = "admin",
    PassHash = HashPassword("admin123")
};

int newId2 = System.Threading.Interlocked.Increment(ref idCounter);
users[newId2] = new UserEntity
{
    Id = newId2,
    Login = "ivanov",
    PassHash = HashPassword("userPass456")
};

app.MapPost("/user", (UserRequestDto request) =>
{
    if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.PassHash))
    {
        return Results.BadRequest(new { error = "Поля Login и PassHash обязательны для заполнения." });
    }

    foreach (var existing in users.Values)
    {
        if (existing.Login.Equals(request.Login, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Conflict(new { error = "Пользователь с логином '" + request.Login + "' уже зарегистрирован." });
        }
    }

    int id = System.Threading.Interlocked.Increment(ref idCounter);
    string passHash = request.PassHash.Length == 64 ? request.PassHash : HashPassword(request.PassHash);

    var newUser = new UserEntity
    {
        Id = id,
        Login = request.Login,
        PassHash = passHash
    };

    users[id] = newUser;
    return Results.Created($"/user/{newUser.Id}", new { newUser.Id, newUser.Login, newUser.PassHash });
});

app.MapGet("/user/{id:int}", (int id) =>
{
    if (users.TryGetValue(id, out var user))
    {
        return Results.Ok(new { user.Id, user.Login, user.PassHash });
    }
    return Results.NotFound(new { error = "Пользователь с ID=" + id + " не найден." });
});

app.MapPut("/user/{id:int}", (int id, UserRequestDto request) =>
{
    if (!users.TryGetValue(id, out var user))
    {
        return Results.NotFound(new { error = "Пользователь с ID=" + id + " не найден." });
    }

    if (!string.IsNullOrWhiteSpace(request.Login))
    {
        foreach (var existing in users.Values)
        {
            if (existing.Id != id && existing.Login.Equals(request.Login, StringComparison.OrdinalIgnoreCase))
            {
                return Results.Conflict(new { error = "Логин '" + request.Login + "' уже занят другим пользователем." });
            }
        }
        user.Login = request.Login;
    }

    if (!string.IsNullOrWhiteSpace(request.PassHash))
    {
        user.PassHash = request.PassHash.Length == 64 ? request.PassHash : HashPassword(request.PassHash);
    }

    users[id] = user;
    return Results.Ok(new { message = "Данные пользователя успешно обновлены.", user.Id, user.Login });
});

app.MapDelete("/user/{id:int}", (int id) =>
{
    if (users.TryRemove(id, out var removed))
    {
        return Results.Ok(new { message = "Пользователь '" + removed.Login + "' с ID=" + id + " успешно удален." });
    }
    return Results.NotFound(new { error = "Пользователь с ID=" + id + " не найден." });
});

app.Run();

public class UserEntity
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PassHash { get; set; } = "";
}

public class UserRequestDto
{
    public string Login { get; set; } = "";
    public string PassHash { get; set; } = "";
}
