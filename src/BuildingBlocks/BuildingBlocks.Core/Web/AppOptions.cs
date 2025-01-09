namespace BuildingBlocks.Core.Web;

/// <summary>
/// Defines application-specific configuration options.
/// </summary>
public class AppOptions
{
    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the API address for the application.
    /// </summary>
    public string? ApiAddress { get; set; }

    /// <summary>
    /// Gets or sets the instance identifier of the application.
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    /// Gets or sets the version of the application.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display a banner.
    /// </summary>
    public bool DisplayBanner { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to display the version.
    /// </summary>
    public bool DisplayVersion { get; set; } = true;

    /// <summary>
    /// Gets or sets the description of the application.
    /// </summary>
    public string? Description { get; set; }
}