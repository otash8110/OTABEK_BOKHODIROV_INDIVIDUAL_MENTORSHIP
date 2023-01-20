using BL.Validation;

namespace BL.Tests
{
    public class InputStringValidationTests
    {
        [Fact]
        public void ValidateCityName_NotEmptyStringPassed_ReturnsFalse()
        {
            var validationService = new ValidationService();

            var result = validationService.ValidateCityName("Tashkent");

            Assert.False(result);
        }

        [Fact]
        public void ValidateCityName_EmptyStringPassed_ReturnsTrue()
        {
            var validationService = new ValidationService();

            var result = validationService.ValidateCityName("");

            Assert.True(result);
        }

        [Fact]
        public void ValidateCityName_NullPassed_ReturnsTrue()
        {
            var validationService = new ValidationService();

            var result = validationService.ValidateCityName(null);

            Assert.True(result);
        }
    }
}
