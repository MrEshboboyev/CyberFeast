namespace BuildingBlocks.Abstractions.Web.Problem;

/// <summary>
/// Defines a method for mapping exceptions to HTTP status codes.
/// </summary>
public interface IProblemDetailMapper
{
    /// <summary>
    /// Gets the HTTP status code mapped to a given exception.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    /// <returns>The HTTP status code corresponding to the exception.</returns>
    int GetMappedStatusCodes(Exception exception);
}