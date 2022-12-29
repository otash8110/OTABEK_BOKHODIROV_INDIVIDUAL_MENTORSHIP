namespace BL.Validation
{
    public class ValidationService : IValidation
    {
        public bool ValidateCityName(string cityName)
        {
            return String.IsNullOrEmpty(cityName);
        }
    }
}
