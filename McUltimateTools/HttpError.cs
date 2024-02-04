namespace McUltimateTools;

public static class HttpError
{
    private static async Task Send(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
    public static async Task Send400(HttpContext context, string message = "Bad Request")
    {
        await Send(context, 400, message);
    }
    public static async Task Send401(HttpContext context, string message = "Unauthorized")
    {
        await Send(context, 401, message);
    }
    public static async Task Send403(HttpContext context, string message = "Forbidden")
    {
        await Send(context, 403, message);
    }
    public static async Task Send404(HttpContext context, string message = "Not Found")
    {
        await Send(context, 404, message);
    }
    public static async Task Send500(HttpContext context, string message = "Internal Server Error")
    {
        await Send(context, 500, message);
    }
}
