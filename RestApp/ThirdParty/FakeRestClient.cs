

using System.Net;

namespace RestApp.ThirdParty
{
    public class FakeRestClient : IRestClient
    {
        private int _callCount = 0;

        public Task<TModel> Get<TModel>(string url)
        {
            _callCount++;
            if (_callCount < 3)
            {
                Console.WriteLine($"[FakeRestClient] Simulating unstable API: throwing WebException on attempt {_callCount}");
                throw new WebException("Simulated network error");
            }
            Console.WriteLine($"[FakeRestClient] Success on attempt {_callCount}");
            return Task.FromResult(default(TModel));
        }

        public Task<TModel> Put<TModel>(string url, TModel model)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> Post<TModel>(string url, TModel model)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> Delete<TModel>(int id)
        {
            throw new NotImplementedException();
        }
    }
}
