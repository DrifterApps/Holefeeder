using DrifterApps.Holefeeder.Budgeting.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Validators
{
    public class CloseAccountValidator : AbstractValidator<CloseAccountCommand>
    {
        public CloseAccountValidator(ILogger<CloseAccountValidator> logger)
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
