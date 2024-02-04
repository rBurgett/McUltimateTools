using System.Text.Json;
using McUltimateTools;

var varCheckRes = VarChecker.Check();

var builder = WebApplication.CreateBuilder(args);

var db = new Db(varCheckRes.UsersTableName);

builder.Services.AddSingleton<ServiceController>(provider => new ServiceController(db));
var app = builder.Build();

app.MapGet("/", async (HttpContext context, ServiceController serviceController) =>
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
        var user = User.Create(userRes.Email);
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
app.MapGet("/users/{id}", async (HttpContext ContextCallback, ServiceController serviceController) =>
{
    try
    {
        var id = ContextCallback.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            ContextCallback.Response.StatusCode = 400;
            await ContextCallback.Response.WriteAsync("Invalid id");
            return;
        }
        var user = await serviceController.db.GetUser(id);
        if (user == null)
        {
            ContextCallback.Response.StatusCode = 404;
            await ContextCallback.Response.WriteAsync("Not found");
            return;
        }
        var json = JsonSerializer.Serialize(user);
        ContextCallback.Response.ContentType = "application/json";
        await ContextCallback.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        ContextCallback.Response.StatusCode = 500;
        await ContextCallback.Response.WriteAsync(e.Message);
    }
});
app.MapDelete("/users/{id}", async (HttpContext ContextCallback, ServiceController serviceController) =>
{
    try
    {
        var id = ContextCallback.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            ContextCallback.Response.StatusCode = 400;
            await ContextCallback.Response.WriteAsync("Invalid id");
            return;
        }
        var deleted = await serviceController.db.DeleteUser(id);
        ContextCallback.Response.StatusCode = 200;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        ContextCallback.Response.StatusCode = 500;
        await ContextCallback.Response.WriteAsync(e.Message);
    }
});
app.Run();
