using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Web.HeaderPropagation;

/// <summary>
/// A message handler builder filter for configuring the header propagation message handler.
/// </summary>
internal class HeaderPropagationMessageHandlerBuilderFilter(
    IOptions<CustomHeaderPropagationOptions> options,
    HeaderPropagationStore header)
    : IHttpMessageHandlerBuilderFilter
{
    private readonly CustomHeaderPropagationOptions _options = options.Value;
    private readonly HeaderPropagationStore _headerPropagationStore = header;

    /// <summary>
    /// Configures the message handler builder to include the header propagation message handler.
    /// </summary>
    /// <param name="next">The next action to configure the builder.</param>
    /// <returns>The configured action for the message handler builder.</returns>
    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            builder.AdditionalHandlers.Add(
                new HeaderPropagationMessageHandler(_options, _headerPropagationStore));

            next(builder);
        };
    }
}