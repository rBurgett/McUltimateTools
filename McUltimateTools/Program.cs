using System.Text.Json;
using McUltimateTools;

var varCheckRes = VarChecker.Check();

var builder = WebApplication.CreateBuilder(args);

var db = new Db(varCheckRes.UsersTableName);

builder.Services.AddSingleton<ServiceController>(provider => new ServiceController(db));
var app = builder.Build();

app.MapGet("/", async (context) =>
{
    await context.Response.WriteAsync("Isaac Fain's Minecraft Ultimate Tools API");
});
app.MapPost("/users", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        UserPostBody? userRes;
        try
        {
            userRes = await JsonSerializer.DeserializeAsync<UserPostBody>(context.Request.Body);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid request body");
            return;
        }
        if (userRes == null || userRes.Email == null || !User.ValidateEmail(userRes.Email))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid request body");
            return;
        }
        if (userRes.Password == null || !User.ValidatePassword(userRes.Password))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Invalid password. Must be a minimum of {Constants.PasswordMinLength} characters and maximum of {Constants.PasswordMaxLength} characters long.");
            return;
        }
        var config = new Argon2Config();
        var salt = CryptoUtil.GenerateSalt(Constants.SaltBytes);
        var passwordHash = CryptoUtil.Argon2Hash(userRes.Password, salt, config);
        var user = User.Create(userRes.Email, passwordHash, salt);
        await serviceController.db.AddUser(user);
        var json = JsonSerializer.Serialize(user);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(e.Message);
    }
});
app.MapGet("/users", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        var users = await serviceController.db.GetUsers();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(users, options);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(e.Message);
    }
});
app.MapGet("/users/{id}", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        var id = context.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid id");
            return;
        }
        var user = await serviceController.db.GetUser(id);
        if (user == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Not found");
            return;
        }
        var json = JsonSerializer.Serialize(user);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(e.Message);
    }
});
app.MapDelete("/users/{id}", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        var id = context.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid id");
            return;
        }
        await serviceController.db.DeleteUser(id);
        context.Response.StatusCode = 200;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(e.Message);
    }
});
app.Run();
