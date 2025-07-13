
namespace RestApp.ThirdParty
{
    public interface IRestClient
    {
        Task<TModel> Get<TModel>(string url);
        Task<TModel> Put<TModel>(string url, TModel model);
        Task<TModel> Post<TModel>(string url, TModel model);
        Task<TModel> Delete<TModel>(int id);
    }
}
