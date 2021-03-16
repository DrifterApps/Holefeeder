using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    public class ModifyAccountCommandHandler : IRequestHandler<ModifyAccountCommand, CommandResult>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<ModifyAccountCommandHandler> _logger;

        public ModifyAccountCommandHandler(IAccountRepository repository, ItemsCache cache, ILogger<ModifyAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }
        public async Task<CommandResult> Handle(ModifyAccountCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            try
            {
                var exists = await _repository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
                if (exists is null)
                {
                    return CommandResult.Create(CommandStatus.NotFound);
                }

                var account = exists with
                {
                    Name = request.Name,
                    Description = request.Description,
                    OpenBalance = request.OpenBalance
                };

                _logger.LogInformation("----- Modify Account - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return CommandResult.Create(CommandStatus.Ok);
            }
            catch (HolefeederDomainException e)
            {
                return CommandResult.Create(CommandStatus.BadRequest, e.Message);
            }
        }
    }
}
