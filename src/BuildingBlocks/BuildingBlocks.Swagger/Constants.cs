namespace BuildingBlocks.Swagger;

/// <summary>
/// Contains constant values used across the application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Contains constants related to API key authentication.
    /// </summary>
    public static class ApiKeyConstants
    {
        /// <summary>
        /// The header name for the API key.
        /// </summary>
        public const string HeaderName = "X-Api-Key";

        /// <summary>
        /// The default scheme name for API key authentication.
        /// </summary>
        public const string DefaultScheme = "ApiKey";

        /// <summary>
        /// The header name for the API version.
        /// </summary>
        public const string HeaderVersion = "api-version";
    }
}