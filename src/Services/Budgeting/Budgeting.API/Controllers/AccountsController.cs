using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class AccountsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_ACCOUNTS = "get-accounts";
            public const string GET_ACCOUNT = "get-account";
            public const string OPEN_ACCOUNT = "open-account";
            public const string CLOSE_ACCOUNT = "close-account";
            public const string FAVORITE_ACCOUNT = "favorite-account";
            public const string MODIFY_ACCOUNT = "modify-account";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Routes.GET_ACCOUNTS, Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccounts([FromQuery] int? offset, int? limit, string[] sort,
            string[] filter, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetAccountsQuery(offset, limit, sort, filter),
                cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_ACCOUNT)]
        [ProducesResponseType(typeof(AccountViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetAccountQuery(id), cancellationToken);

            if (response is null)
            {
                return NotFound();
            } 
            return Ok(response);
        }

        [HttpPost(Routes.OPEN_ACCOUNT, Name = Routes.OPEN_ACCOUNT)]
        [ProducesResponseType(typeof(CommandResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> OpenAccountCommand([FromBody] OpenAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Created)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute(Routes.GET_ACCOUNT, new {id = result.Result}, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ex.Errors.Select(e => e.ToString())));
            }
        }

        [HttpPut(Routes.CLOSE_ACCOUNT, Name = Routes.CLOSE_ACCOUNT)]
        [ProducesResponseType(typeof(CommandResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CloseAccountCommand([FromBody] CloseAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Ok)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ex.Errors.Select(e => e.ToString())));
            }
        }

        [HttpPut(Routes.FAVORITE_ACCOUNT, Name = Routes.FAVORITE_ACCOUNT)]
        [ProducesResponseType(typeof(CommandResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> FavoriteAccountCommand([FromBody] FavoriteAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Ok)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ex.Errors.Select(e => e.ToString())));
            }
        }

        [HttpPut(Routes.MODIFY_ACCOUNT, Name = Routes.MODIFY_ACCOUNT)]
        [ProducesResponseType(typeof(CommandResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ModifyAccountCommand([FromBody] ModifyAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Ok)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ex.Errors.Select(e => e.ToString())));
            }
        }
    }
}
