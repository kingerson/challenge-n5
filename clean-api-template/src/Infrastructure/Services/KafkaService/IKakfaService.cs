namespace MsClean.Infrastructure;
using System.Threading.Tasks;

public interface IKakfaService
{
    Task<bool> ProduceAsync(string topic, string message);
}
