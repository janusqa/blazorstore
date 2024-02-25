
namespace BlazorStore.ApiAccess.Service
{
    public class UnitOfWork : IUnitOfWork
    {

        public UnitOfWork(IHttpClientFactory httpClient)
        {

        }

        public void Dispose()
        {
        }

    }
}