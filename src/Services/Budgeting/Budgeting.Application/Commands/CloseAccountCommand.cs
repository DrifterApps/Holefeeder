using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    public record CloseAccountCommand : IRequest<CommandResult>
    {
        public Guid Id { get; init; }
    }
}
