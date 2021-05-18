using System;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetCategoriesQuery : IRequest<CategoryViewModel[]>
    {
    }
}
