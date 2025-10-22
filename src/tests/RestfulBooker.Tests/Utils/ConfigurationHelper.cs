using Microsoft.Extensions.Configuration;

namespace RestfulBooker.Tests.Utils;

public static class ConfigurationHelper
{
    private static IConfigurationRoot? _config;

    public static IConfigurationRoot Config => _config ??= new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(Path.Combine("Resources", "Config", "appsettings.json"), optional: false, reloadOnChange: true)
        .Build();

}
