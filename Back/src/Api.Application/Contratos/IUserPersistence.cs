using System.Threading.Tasks;
using Api.Domain.Identity;

namespace Api.Application.Contratos
{
    public interface IUserPersistence
    {
        Task<User> GetUserByUserNameAsync(string username);
    }
}
