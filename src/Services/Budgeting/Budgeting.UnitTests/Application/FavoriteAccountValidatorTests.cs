using System;

using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class FavoriteCommandValidatorTests
    {
        private readonly ILogger<FavoriteAccountValidator> _logger;

        public FavoriteCommandValidatorTests()
        {
            _logger = Substitute.For<ILogger<FavoriteAccountValidator>>();
        }

        [Fact]
        public void GivenFavoriteAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
        {
            var validator = new FavoriteAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Id,
                new FavoriteAccountCommand {Id = Guid.Empty, IsFavorite = true});
        }
    }
}
