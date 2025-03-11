using BuildingBlocks.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace Tests.Shared.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfigurationRoot _configurationRoot;

    static ConfigurationHelper()
    {
        _configurationRoot = BuildConfiguration();
    }

    public static TOptions BindOptions<TOptions>()
        where TOptions : new()
    {
        return _configurationRoot.BindOptions<TOptions>();
    }

    public static IConfigurationRoot BuildConfiguration()
    {
        var rootPath = Directory.GetCurrentDirectory();

        return new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
