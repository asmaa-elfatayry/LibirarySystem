using Application.Common;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<ApplicationUser>> RegisterAsync(string email, string password, string fullName);
}
