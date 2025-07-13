
using RestApp.ThirdParty;

namespace RestApp
{
    public class RestAppClassThatUsesRestClient
    {
        private IRestClient _3rdPartyRestClient;
        private ILogger _logger;

        public RestAppClassThatUsesRestClient(IRestClient restClient, ILogger logger)
        {
            _3rdPartyRestClient = restClient;
            _logger = logger;
        }

        public Task<TModel> GetSomething<TModel>(string url)
        {
            return _3rdPartyRestClient.Get<TModel>(url);
        }
    }
}
