
using Microsoft.Extensions.Configuration;

namespace MongoDB.SimpleRepository.Tests
{
    public class Config
    {
        public static IConfigurationRoot Settings;

        public Config(IConfigurationRoot settings)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",
                             optional: false,
                             reloadOnChange: true)
                .AddEnvironmentVariables();

                builder.AddUserSecrets<Startup>();

            Settings = builder.Build();
        }
    }
}
