﻿using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    public class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, CommandResult<Guid>>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<OpenAccountCommandHandler> _logger;

        public OpenAccountCommandHandler(IAccountRepository repository, ItemsCache cache, ILogger<OpenAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }
        public async Task<CommandResult<Guid>> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            try
            {
                var exists = await _repository.FindByNameAsync(request.Name, (Guid)_cache["UserId"], cancellationToken);
                if (exists is not null)
                {
                    throw new ValidationException($"Account name '{request.Name}' already exists");
                }
                
                var account = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
                    request.Description, (Guid)_cache["UserId"]);

                _logger.LogInformation("----- Opening Account - Account: {@Account}", account);

                await _repository.CreateAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return CommandResult<Guid>.Create(CommandStatus.Created, account.Id);
            }
            catch (HolefeederDomainException e)
            {
                return CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty, e.Message);
            }
        }
    }
}