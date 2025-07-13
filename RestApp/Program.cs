using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestApp;
using RestApp.ThirdParty;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) 
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();

        services.Configure<RestClientRetryOptions>(config.GetSection("RestClientRetry"));
        services.AddSingleton<ILogger, Logger>(); 

        services.AddSingleton<IRestClient, FakeRestClient>();

        services.AddSingleton<IRestClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<RestClientRetryOptions>>().Value;

            return new RestClientWithRetry(
                new FakeRestClient(),
                provider.GetRequiredService<ILogger>(),
                maxRetries: options.MaxRetries,
                retryDelay: TimeSpan.FromSeconds(options.RetryDelaySeconds)
            );
        });

        services.AddSingleton<RestAppClassThatUsesRestClient>();

        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<RestAppClassThatUsesRestClient>();

        Console.WriteLine("Calling GetSomething...");
        await app.GetSomething<object>("http://unstable-api.com");
        Console.WriteLine("Done.");
    }
}
