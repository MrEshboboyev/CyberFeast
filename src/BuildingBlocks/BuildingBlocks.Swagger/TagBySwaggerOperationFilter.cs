using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Tags OpenAPI operations based on SwaggerOperation attributes.
/// </summary>
public class TagBySwaggerOperationFilter : IOperationFilter
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

        // Retrieve tags from SwaggerOperationAttribute applied to the controller or endpoint.
        var swaggerOperationAttribute = controllerActionDescriptor
            .ControllerTypeInfo.GetCustomAttributes(typeof(SwaggerOperationAttribute), true)
            .Cast<SwaggerOperationAttribute>()
            .FirstOrDefault();

        if (swaggerOperationAttribute != null && swaggerOperationAttribute.Tags.Length != 0)
        {
            operation.Tags = swaggerOperationAttribute.Tags.Select(
                x => new OpenApiTag
                {
                    Name = x
                }).ToList();
        }

        if (controllerActionDescriptor.EndpointMetadata
                .FirstOrDefault(x => x is SwaggerOperationAttribute) is SwaggerOperationAttribute swaggerOperationEndpoint
            && swaggerOperationEndpoint.Tags.Length != 0)
        {
            operation.Tags = swaggerOperationEndpoint.Tags.Select(
                x => new OpenApiTag
                {
                    Name = x
                }).ToList();
        }
    }
}