using DAL.Identity;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.UserPasswordValidation
{
    public class UserPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userEmail = context.UserName;
            var userPassword = context.Password;

            var user = await userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                var isPasswordCorrect = await userManager.CheckPasswordAsync(user, userPassword);
                if (!isPasswordCorrect)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient,
                        "Invalid credentials");
                    return;
                }
                else
                {
                    var role = await userManager.GetRolesAsync(user);
                    context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: "custom",
                    claims: new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, string.Join(',', role))
                    });

                    return;
                }
            }

            return;
        }
    }
}
