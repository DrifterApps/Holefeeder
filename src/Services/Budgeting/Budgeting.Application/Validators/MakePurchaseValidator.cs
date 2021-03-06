﻿using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using FluentValidation;

namespace DrifterApps.Holefeeder.Budgeting.Application.Validators
{
    public class MakePurchaseValidator : AbstractValidator<MakePurchaseCommand>
    {
        public MakePurchaseValidator()
        {
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
        }
    }
}
