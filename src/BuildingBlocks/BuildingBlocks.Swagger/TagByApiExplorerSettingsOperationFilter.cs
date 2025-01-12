using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Tags OpenAPI operations based on ApiExplorerSettings attributes.
/// </summary>
public class TagByApiExplorerSettingsOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the filter to the specified operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to modify.</param>
    /// <param name="context">The context for the operation filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
            return;

        // Retrieve group name from ApiExplorerSettingsAttribute applied to the controller or endpoint.
        var apiExplorerSettings = controllerActionDescriptor
            .ControllerTypeInfo.GetCustomAttributes(typeof(ApiExplorerSettingsAttribute), true)
            .Cast<ApiExplorerSettingsAttribute>()
            .FirstOrDefault();

        if (apiExplorerSettings != null && !string.IsNullOrWhiteSpace(apiExplorerSettings.GroupName))
        {
            operation.Tags = new List<OpenApiTag>
            {
                new() { Name = apiExplorerSettings.GroupName }
            };
        }

        var apiExplorerSettingsEndpoint = controllerActionDescriptor.EndpointMetadata
            .FirstOrDefault(x => x is ApiExplorerSettingsAttribute) as ApiExplorerSettingsAttribute;

        if (apiExplorerSettingsEndpoint != null)
        {
            operation.Tags = new List<OpenApiTag>
            {
                new() { Name = apiExplorerSettingsEndpoint.GroupName }
            };
        }
    }
}