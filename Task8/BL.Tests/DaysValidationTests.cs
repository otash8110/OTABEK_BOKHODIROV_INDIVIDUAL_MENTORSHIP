using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BL.Tests
{
    public class DaysValidationTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ValidateMinMaxDays_WhenCalled_ReturnsTrue(int days)
        {
            var validationService = new ValidationService();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["MinDays"]).Returns("1");
            mockConfiguration.SetupGet(x => x["MaxDays"]).Returns("5");

            var result = validationService.ValidateMinMaxDays(days, mockConfiguration.Object);

            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void ValidateMinMaxDays_WhenWrongDataPassed_ReturnsFalse(int days)
        {
            var validationService = new ValidationService();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["MinDays"]).Returns("1");
            mockConfiguration.SetupGet(x => x["MaxDays"]).Returns("5");

            var result = validationService.ValidateMinMaxDays(days, mockConfiguration.Object);

            Assert.False(result);
        }
    }
}
