namespace BuildingBlocks.Web;

/// <summary>
/// Represents the configuration options for Cross-Origin Resource Sharing (CORS).
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Gets or sets the allowed URLs for CORS.
    /// </summary>
    public IEnumerable<string> AllowedUrls { get; set; }
}