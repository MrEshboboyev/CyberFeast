using BuildingBlocks.Web.Problem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace FoodDelivery.Services.Identity.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomProblemDetails(problemDetailsOptions =>
        {
            // customization problem details should go here
            problemDetailsOptions.CustomizeProblemDetails = problemDetailContext =>
            {
                if (problemDetailContext.HttpContext.Features.Get<IExceptionHandlerFeature>() is not null)
                { }
            };
        });

        return builder;
    }
}
