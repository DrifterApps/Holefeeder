using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record CategoryViewModel(Guid Id, string Name, string Color, decimal BudgetAmount, bool Favorite)
    {
    }
}
