using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Security;

/// <summary>
/// Represents a 403 Forbidden error.
/// </summary>
public class ForbiddenProblemDetails : ProblemDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenProblemDetails"/> class.
    /// </summary>
    /// <param name="details">Optional detailed error message.</param>
    public ForbiddenProblemDetails(string? details = null)
    {
        Title = "ForbiddenException";
        Detail = details;
        Status = 403;
    }
}