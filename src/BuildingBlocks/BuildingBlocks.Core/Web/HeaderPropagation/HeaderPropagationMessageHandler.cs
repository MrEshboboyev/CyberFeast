using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Core.Web.HeaderPropagation;

/// <summary>
/// A message handler for propagating headers from the <see cref="HeaderPropagationStore"/> to outgoing HTTP requests.
/// </summary>
public class HeaderPropagationMessageHandler(
    CustomHeaderPropagationOptions options,
    HeaderPropagationStore headers
) : DelegatingHandler
{
    private readonly CustomHeaderPropagationOptions _options = options;
    private readonly HeaderPropagationStore _headerPropagationStore = headers;

    /// <summary>
    /// Sends an HTTP request with propagated headers asynchronously.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the HTTP response message as the result.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        foreach (var headerName in options.HeaderNames)
        {
            // Get the incoming header value
            headers.Headers.TryGetValue(headerName, out var headerValue);
            if (StringValues.IsNullOrEmpty(headerValue))
            {
                continue;
            }

            request.Headers.TryAddWithoutValidation(headerName, headerValue.ToArray());
        }

        return base.SendAsync(request, cancellationToken);
    }
}