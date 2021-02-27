using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Validators;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class OpenCommandValidatorTests
    {
        private readonly ILogger<OpenAccountValidator> _logger;
        
        public OpenCommandValidatorTests()
        {
            _logger = Substitute.For<ILogger<OpenAccountValidator>>();
        }
        
        [Theory, MemberData(nameof(InvalidNames))]
        public void GivenOpenAccountValidator_WhenNameIsInvalid_ThenShouldHaveError(string name)
        {
            var validator = new OpenAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Name, name);
        }

        [Theory, MemberData(nameof(InvalidDateTimes))]
        public void GivenOpenAccountValidator_WhenOpenDateIsInvalid_ThenShouldHaveError(DateTime openDate)
        {
            var validator = new OpenAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.OpenDate, openDate);
        }

        [Theory, MemberData(nameof(InvalidTypes))]
        public void GivenOpenAccountValidator_WhenTypeIsInvalid_ThenShouldHaveError(AccountType type)
        {
            var validator = new OpenAccountValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Type, type);
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
        
        public static IEnumerable<object[]> InvalidDateTimes
        {
            get
            {
                yield return new object[]{ DateTime.MinValue };
                yield return new object[]{ default(DateTime) };
                yield return new object[]{ null };
            }
        }
        
        public static IEnumerable<object[]> InvalidTypes
        {
            get
            {
                yield return new object[]{ default(AccountType) };
                yield return new object[]{ null };
            }
        }
    }
}
