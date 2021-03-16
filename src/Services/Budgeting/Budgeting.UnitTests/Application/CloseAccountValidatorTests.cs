using System;

using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class CloseCommandValidatorTests
    {
        private readonly ILogger<CloseAccountValidator> _logger;
        
        public CloseCommandValidatorTests()
        {
            _logger = Substitute.For<ILogger<CloseAccountValidator>>();
        }
        
        [Fact]
        public void GivenCloseAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
        {
            var validator = new CloseAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Id, new CloseAccountCommand{Id = Guid.Empty});
        }
        
        [Fact]
        public void GivenCloseAccountValidator_WhenIdIsNull_ThenShouldHaveError()
        {
            var validator = new CloseAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Id, new CloseAccountCommand());
        }
    }
}
