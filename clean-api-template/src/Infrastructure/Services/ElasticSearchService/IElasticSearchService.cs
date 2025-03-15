namespace MsClean.Infrastructure;
using System.Threading.Tasks;
using MsClean.Domain;

public interface IElasticSearchService<T> where T : Entity
{
    Task<bool> IndexAsync(T model, string index);
    Task<bool> IndexBulkAsync(IEnumerable<T> models, string index);
}
