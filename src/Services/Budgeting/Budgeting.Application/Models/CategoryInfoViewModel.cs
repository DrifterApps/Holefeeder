using System;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record CategoryInfoViewModel(Guid Id, string Name, CategoryType Type, string Color)
    {
    }
}
