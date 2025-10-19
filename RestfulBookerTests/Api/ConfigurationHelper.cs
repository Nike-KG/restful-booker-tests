using Microsoft.Extensions.Configuration;

namespace RestfulBookerTests.Api;

public  static class ConfigurationHelper
{
    private static IConfigurationRoot? _config;

    public static IConfigurationRoot Config => _config ??= new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("Resources\\appsettings.json", optional: false, reloadOnChange: true)
        .Build();
        
    }

