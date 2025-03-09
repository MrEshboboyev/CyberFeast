using BuildingBlocks.Web.Problem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace FoodDelivery.Services.Orders.Shared.Extensions.WebApplicationBuilderExtensions;

internal static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomProblemDetails(problemDetailsOptions =>
        {
            // customization problem details should go here
            problemDetailsOptions.CustomizeProblemDetails = problemDetailContext =>
            {
                // with help of capture exception middleware for capturing actual exception
                // .net 8 will add `IExceptionHandlerFeature`in `DisplayExceptionContent` and `SetExceptionHandlerFeatures` methods `DeveloperExceptionPageMiddlewareImpl` class, exact functionality of CaptureException
                // bet before .net 8 preview 5 we should add `IExceptionHandlerFeature` manually with our `UseCaptureException`
                if (problemDetailContext.HttpContext.Features.Get<IExceptionHandlerFeature>() is not null)
                { }
            };
        });

        return builder;
    }
}