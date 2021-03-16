using DrifterApps.Holefeeder.Budgeting.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Validators
{
    public class FavoriteAccountValidator : AbstractValidator<FavoriteAccountCommand>
    {
        public FavoriteAccountValidator(ILogger<FavoriteAccountValidator> logger)
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
