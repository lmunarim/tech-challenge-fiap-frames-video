using System.Threading.Tasks;

namespace fiap.Domain.Interfaces
{
    public interface ISecretManagerService
    {
        Task<T> ObterSecret<T>(string segredo);
    }
}
