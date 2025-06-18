using Microsoft.Extensions.Configuration;

namespace AgenticAI;

public static class ConfigurationUtils
{
    private static IConfigurationRoot? _configuration;
    public static IConfigurationRoot Configuration
    {
        get
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();
            }

            return _configuration;
        }
    }

    public static string GetConfigurationValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Configuration key cannot be null or empty.", nameof(key));
        }

        var configurationValue = Configuration[key];

        if (configurationValue == null)
        {
            throw new KeyNotFoundException($"Configuration key '{key}' not found.");
        }

        return configurationValue;
    }
}