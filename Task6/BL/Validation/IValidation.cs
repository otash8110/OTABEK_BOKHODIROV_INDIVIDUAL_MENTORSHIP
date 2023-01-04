namespace BL.Validation
{
    public interface IValidation
    {
        bool ValidateCityName(string cityName);
        bool ValidateMinMaxDays(int days);
    }
}
