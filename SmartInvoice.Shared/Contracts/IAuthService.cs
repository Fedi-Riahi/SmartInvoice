using System.Threading.Tasks;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Shared.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    }
}