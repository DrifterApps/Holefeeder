using DrifterApps.Holefeeder.Budgeting.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Validators
{
    public class OpenAccountValidator : AbstractValidator<OpenAccountCommand>
    {
        public OpenAccountValidator(ILogger<OpenAccountValidator> logger)
        {
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
            RuleFor(command => command.OpenDate).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
