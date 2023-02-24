using IdentityModel.Client;

namespace API.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope, string email, string password);
    }
}
