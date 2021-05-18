using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

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
    public class TransactionsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_TRANSACTIONS = "get-transactions";
            public const string GET_TRANSACTION = "get-transaction";
        }

        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
        }

        [HttpGet(Routes.GET_TRANSACTIONS, Name = Routes.GET_TRANSACTIONS)]
        [ProducesResponseType(typeof(TransactionViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransactions([FromQuery] int? offset, int? limit, string[] sort,
            string[] filter, CancellationToken cancellationToken = default)
        {
            var (totalCount, transactionViewModels) = await _mediator.Send(new GetTransactionsQuery(offset, limit, sort, filter),
                cancellationToken);

            Response?.Headers?.Add("X-Total-Count", $"{totalCount}");

            return Ok(transactionViewModels);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransaction(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetTransactionQuery {Id = id}, cancellationToken);

            return Ok(response);
        }
    }
}
