using System.Threading.Tasks;
using Jwt.Identity.Framework.DataAnnotations;
using Xunit;

namespace Jwt.Identity.Test
{
    public class ValidationAttributeTest
    {
        [Theory]
        [InlineData("09123406285", true)]
        [InlineData("09903406285", true)]
        [InlineData("09203406285", true)]
        [InlineData("9123406285", true)]
        [InlineData("+989123406285", true)]
        [InlineData("989413406285", true)]
        [InlineData("9413406285", true)]
        [InlineData("413406285", false)]
        [InlineData("+9213406285", false)]
        public async Task MobileNoValidationAttribute(string mobileNo, bool expectedValid)
        {
            var attrbute = new MobileNoAttribute();

            //act
            var actuleValid = attrbute.IsValid(mobileNo);
            // Assert
            Assert.Equal(expectedValid, actuleValid);
        }
    }
}