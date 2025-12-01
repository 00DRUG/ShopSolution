using AutoFixture;
using FluentValidation.TestHelper;
using Shop.Application.DTOs;
using Shop.Application.Validators;

namespace Shop.Tests.ApplicationTests
{
    public class CreateProductValidatorTests
    {
        private readonly CreateProductValidator _validator;
        private readonly Fixture _fixture;

        public CreateProductValidatorTests()
        {
            _validator = new CreateProductValidator();
            _fixture = new Fixture();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            // Name empty
            var model = _fixture.Build<CreateProductDto>()
                                .With(x => x.Name, string.Empty)
                                .Create();

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Url_Is_Invalid()
        {
            // Arrange
            var model = _fixture.Build<CreateProductDto>()
                                .With(x => x.ImgUrl, "this-is-not-a-url")
                                .Create();

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ImgUrl);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Data_Is_Valid()
        {
            // Arrange
            var model = new CreateProductDto("Valid Name", "https://google.com");

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}