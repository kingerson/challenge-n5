namespace MsClean.Infrastructure;
using System.Threading.Tasks;

public interface IHttpService
{
    Task<T> GetAsync<T>(string url);
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);
}
