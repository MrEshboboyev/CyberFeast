using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Security;

/// <summary>
/// Represents a 401 Unauthorized error.
/// </summary>
public class UnauthorizedProblemDetails : ProblemDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedProblemDetails"/> class.
    /// </summary>
    /// <param name="details">Optional detailed error message.</param>
    public UnauthorizedProblemDetails(string? details = null)
    {
        Title = "UnauthorizedException";
        Detail = details;
        Status = 401;
    }
}