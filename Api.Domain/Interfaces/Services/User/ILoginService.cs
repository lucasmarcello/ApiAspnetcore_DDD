using System.Threading.Tasks;
using Api.Domain.DTO;

namespace Api.Domain.Interfaces.Services.User
{
    public interface ILoginService
    {
        Task<object> FindByLogin(LoginDTO user);
    }
}
