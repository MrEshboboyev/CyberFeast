using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents an HTTP response exception with status code, response content, and headers.
/// </summary>
public class HttpResponseException : CustomException
{
    /// <summary>
    /// Gets the response content of the exception.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the headers associated with the exception.
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResponseException"/> class with a specified response content, status code, headers, and inner exception.
    /// </summary>
    /// <param name="responseContent">The response content.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="headers">The headers associated with the exception.</param>
    /// <param name="inner">The inner exception.</param>
    public HttpResponseException(
        string responseContent,
        int statusCode = StatusCodes.Status500InternalServerError,
        IReadOnlyDictionary<string, IEnumerable<string>>? headers = null,
        System.Exception? inner = null)
        : base(responseContent, statusCode, inner)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        Headers = headers;
    }

    /// <summary>
    /// Returns a string representation of the exception.
    /// </summary>
    /// <returns>The string representation of the exception.</returns>
    public override string ToString()
    {
        return $"HTTP Response: \n\n{ResponseContent}\n\n{base.ToString()}";
    }
}