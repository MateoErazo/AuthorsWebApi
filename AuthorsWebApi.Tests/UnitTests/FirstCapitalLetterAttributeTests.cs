using AuthorsWebApi.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Tests.UnitTests
{
    [TestClass]
    public class FirstCapitalLetterAttributeTests
    {
        [TestMethod]
        public void FirstLowercaseLetter_ReturnsAnError()
        {
            //preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = "jesus";
            var valContext = new ValidationContext(new {Name = value});

            //execution
            ValidationResult result = firstCapitalLetter.GetValidationResult(value, valContext);

            //verification
            Assert.AreEqual("The first letter should be uppercase.", result.ErrorMessage);
        }

        [TestMethod]
        public void NullValue_DoesNotReturnError()
        {
            //preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = null;
            var valContext = new ValidationContext(new { Name = value });

            //execution
            ValidationResult result = firstCapitalLetter.GetValidationResult(value, valContext);

            //verification
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FirstCapitalLetter_DoesNotReturnError()
        {
            //preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = "Jesus";
            var valContext = new ValidationContext(new { Name = value });

            //execution
            ValidationResult result = firstCapitalLetter.GetValidationResult(value, valContext);

            //verification
            Assert.IsNull(result);
        }
    }
}