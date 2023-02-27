using IdentityModel.Client;

namespace API.Services
{
    public class TokenService : ITokenService
    {

        public TokenService()
        {

        }

        public async Task<TokenResponse> GetToken(string scope, string email, string password)
        {
            var ur = Environment.GetEnvironmentVariable("ISURL");
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(handler);
            var discoveryDocument = await client.GetDiscoveryDocumentAsync($"https://{ur}:10980");
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                UserName = email,
                Password = password,
                Scope = scope,
                ClientId = "client",
                ClientSecret = "secret",
                Address = discoveryDocument.TokenEndpoint
            });
            if (tokenResponse.IsError)
            {
                throw new Exception("Token Error");
            }
            return tokenResponse;
        }
    }
}
