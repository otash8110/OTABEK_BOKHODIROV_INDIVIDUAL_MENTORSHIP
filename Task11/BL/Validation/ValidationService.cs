using Microsoft.Extensions.Configuration;

namespace BL.Validation
{
    public class ValidationService : IValidation
    {
        public bool ValidateCityName(string cityName)
        {
            return String.IsNullOrEmpty(cityName);
        }

        public bool ValidateMinMaxDays(int days, IConfiguration configuration)
        {
            var minDays = Convert.ToInt32(configuration["MinDays"]);
            var maxDays = Convert.ToInt32(configuration["MaxDays"]);

            return days >= minDays && days <= maxDays;
        }
    }
}
