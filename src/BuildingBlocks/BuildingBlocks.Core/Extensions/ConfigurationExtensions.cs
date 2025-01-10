using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for the IConfiguration interface to bind configuration sections to options.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Binds a configuration section to an instance of the specified options type <typeparamref name="TOptions"/>.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="section">The configuration section to bind.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>An instance of the specified options type.</returns>
    public static TOptions BindOptions<TOptions>(
        this IConfiguration configuration,
        string section,
        Action<TOptions>? configurator = null
    )
        where TOptions : new()
    {
        var options = new TOptions();
        var optionsSection = configuration.GetSection(section);
        optionsSection.Bind(options);
        configurator?.Invoke(options);
        return options;
    }

    /// <summary>
    /// Binds a configuration section to an instance of the specified options type <typeparamref name="TOptions"/>.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>An instance of the specified options type.</returns>
    public static TOptions BindOptions<TOptions>(
        this IConfiguration configuration,
        Action<TOptions>? configurator = null
    )
        where TOptions : new()
    {
        return BindOptions(configuration, typeof(TOptions).Name, configurator);
    }
}