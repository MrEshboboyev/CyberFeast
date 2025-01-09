using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="HttpResponseMessage"/> class.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Throws an exception if the IsSuccessStatusCode property for the HTTP response is <see langword="false"/>,
    /// and reads exception detail from response content. This method provides more detailed exceptions compared to the default <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/>.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    public static async Task EnsureSuccessStatusCodeWithDetailAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();

        throw new HttpResponseException(content, (int)response.StatusCode);
    }
}