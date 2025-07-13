
namespace RestApp
{
    public class RestClientRetryOptions
    {
        public int MaxRetries { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 1;
    }
}
