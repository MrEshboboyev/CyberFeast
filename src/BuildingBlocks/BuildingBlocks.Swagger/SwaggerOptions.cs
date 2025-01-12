namespace BuildingBlocks.Swagger;

/// <summary>
/// Represents the configuration options for Swagger.
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// Gets or sets the title of the Swagger documentation.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the name of the Swagger documentation.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the version of the Swagger documentation.
    /// </summary>
    public string? Version { get; set; }
}