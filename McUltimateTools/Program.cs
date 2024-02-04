using System.Text.Json;
using McUltimateTools;

var varCheckRes = VarChecker.Check();

var builder = WebApplication.CreateBuilder(args);

var db = new Db(varCheckRes.UsersTableName, varCheckRes.SessionTokensTableName);

builder.Services.AddSingleton<ServiceController>(provider => new ServiceController(db));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();

app.MapGet("/", async (HttpContext context, ServiceController serviceController) =>
{
    await context.Response.WriteAsync("Isaac Fain's Minecraft Ultimate Tools API");
});
app.MapPost("/login", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        LoginPostBody? loginRes;
        try
        {
            loginRes = await JsonSerializer.DeserializeAsync<LoginPostBody>(context.Request.Body);
        }
        catch (Exception)
        {
            await HttpError.Send400(context);
            return;
        }
        if (loginRes == null || loginRes.Email == null || loginRes.Password == null)
        {
            await HttpError.Send400(context);
            return;
        }
        var user = await serviceController.db.FindUserByEmail(loginRes.Email);
        if (user == null)
        {
            await HttpError.Send401(context);
            return;
        }
        var config = new Argon2Config();
        var passwordHash = CryptoUtil.Argon2Hash(loginRes.Password, user.PasswordSalt, config);
        if (passwordHash != user.PasswordHash)
        {
            await HttpError.Send401(context);
            return;
        }
        var token = Util.GenerateUuid();
        var thirtyDaysFromNow = DateTime.UtcNow.AddDays(Constants.SessionTokenExpirationDays);
        var expiration = Util.ToIsoDateString(thirtyDaysFromNow);
        var sessionToken = new SessionToken(token, user.Id, expiration);
        await serviceController.db.AddSessionToken(sessionToken);
        var json = JsonSerializer.Serialize(sessionToken);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        await HttpError.Send500(context);
    }
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
            await HttpError.Send400(context);
            return;
        }
        if (userRes == null || userRes.Email == null || !User.ValidateEmail(userRes.Email))
        {
            await HttpError.Send400(context);
            return;
        }
        if (userRes.Password == null || !User.ValidatePassword(userRes.Password))
        {
            await HttpError.Send400(context);
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
        await HttpError.Send500(context);
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
        await HttpError.Send500(context);
    }
});
app.MapGet("/users/{id}", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        var id = context.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            await HttpError.Send400(context);
            return;
        }
        var user = await Util.GetUserFromToken(serviceController.db, context);
        if (user == null || user.Id != id)
        {
            await HttpError.Send401(context);
            return;
        }
        var json = JsonSerializer.Serialize(user);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        await HttpError.Send500(context);
    }
});
app.MapDelete("/users/{id}", async (HttpContext context, ServiceController serviceController) =>
{
    try
    {
        var id = context.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            await HttpError.Send404(context);
            return;
        }
        var user = await Util.GetUserFromToken(serviceController.db, context);
        if (user == null || user.Id != id)
        {
            await HttpError.Send401(context);
            return;
        }
        await serviceController.db.DeleteUser(id);
        context.Response.StatusCode = 200;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        await HttpError.Send500(context);
    }
});
app.Run();
