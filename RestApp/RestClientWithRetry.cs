
using RestApp.ThirdParty;
using System.Net;

namespace RestApp
{
    public class RestClientWithRetry : IRestClient
    {
        private readonly IRestClient _inner;
        private readonly ILogger _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryDelay;

        public RestClientWithRetry(IRestClient inner, ILogger logger, int maxRetries = 3, TimeSpan? retryDelay = null)
        {
            _inner = inner;
            _logger = logger;
            _maxRetries = maxRetries;
            _retryDelay = retryDelay ?? TimeSpan.FromSeconds(1);
        }

        public async Task<TModel> Get<TModel>(string url)
        {
            return await ExecuteWithRetry(() => _inner.Get<TModel>(url));
        }

        public async Task<TModel> Put<TModel>(string url, TModel model)
        {
            return await ExecuteWithRetry(() => _inner.Put<TModel>(url, model));
        }

        public async Task<TModel> Post<TModel>(string url, TModel model)
        {
            return await ExecuteWithRetry(() => _inner.Post<TModel>(url, model));
        }

        public async Task<TModel> Delete<TModel>(int id)
        {
            return await ExecuteWithRetry(() => _inner.Delete<TModel>(id));
        }

        private async Task<TModel> ExecuteWithRetry<TModel>(Func<Task<TModel>> operation)
        {
            int attempt = 0;
            Exception lastException = null;

            while (attempt < _maxRetries)
            {
                try
                {
                    return await operation();
                }
                catch (WebException ex)
                {
                    lastException = ex;
                    attempt++;
                    if (attempt >= _maxRetries)
                    {
                        _logger.Error(ex);
                        return default;
                    }
                    await Task.Delay(_retryDelay);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }
            throw lastException;
        }
    }
}
