namespace FoodDelivery.Services.Identity.Api.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRevokeAccessTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RevokeAccessTokenMiddleware>();
    }
}