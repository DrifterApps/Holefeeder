using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record QueryResults<T>(int TotalCount, IEnumerable<T> Items)
    {
    }
}
