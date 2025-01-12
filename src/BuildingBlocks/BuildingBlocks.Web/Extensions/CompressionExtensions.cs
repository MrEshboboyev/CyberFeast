using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Provides extension methods for adding response compression to an ASP.NET Core application.
/// </summary>
public static class CompressionExtensions
{
    /// <summary>
    /// Adds response compression services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCompression(this WebApplicationBuilder builder)
    {
        // Configure response compression options
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true; // Enable compression for HTTPS responses
        });

        // Configure Gzip compression provider
        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest; // Set Gzip compression level to fastest
        });

        // Configure Brotli compression provider
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest; // Set Brotli compression level to fastest
        });

        return builder;
    }
}