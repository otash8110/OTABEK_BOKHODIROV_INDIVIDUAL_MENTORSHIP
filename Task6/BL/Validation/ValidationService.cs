using Microsoft.Extensions.Configuration;

namespace BL.Validation
{
    public class ValidationService : IValidation
    {
        private readonly IConfiguration configuration;

        public ValidationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public bool ValidateCityName(string cityName)
        {
            return String.IsNullOrEmpty(cityName);
        }

        public bool ValidateMinMaxDays(int days)
        {
            var minDays = Convert.ToInt32(configuration["MinDays"]);
            var maxDays = Convert.ToInt32(configuration["MaxDays"]);

            return days >= minDays && days <= maxDays;
        }
    }
}
