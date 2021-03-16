using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class ModifyAccountCommandValidatorTests
    {
        private readonly ILogger<ModifyAccountValidator> _logger;
        
        public ModifyAccountCommandValidatorTests()
        {
            _logger = Substitute.For<ILogger<ModifyAccountValidator>>();
        }
        
        [Fact]
        public void GivenModifyAccountValidator_WhenIdIsInvalid_ThenShouldHaveError()
        {
            var validator = new ModifyAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Id, Guid.Empty);
        }
        
        [Theory, MemberData(nameof(InvalidNames))]
        public void GivenModifyAccountValidator_WhenNameIsInvalid_ThenShouldHaveError(string name)
        {
            var validator = new ModifyAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Name, name);
        }
        
        private const string longString
            = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*(){}|[]\\;':\",./<>?";
        
        public static IEnumerable<object[]> InvalidNames
        {
            get
            {
                yield return new object[]{ "" };
                yield return new object[]{ "           " };
                yield return new object[]{ string.Concat(longString, longString, longString) };
                yield return new object[]{ null };
            }
        }
    }
}
