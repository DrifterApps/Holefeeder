using DrifterApps.Holefeeder.Budgeting.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Validators
{
    public class ModifyAccountValidator : AbstractValidator<ModifyAccountCommand>
    {
        public ModifyAccountValidator(ILogger<ModifyAccountValidator> logger)
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
