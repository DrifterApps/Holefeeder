using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
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
    public class CashflowsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_UPCOMING = "get-upcoming";
            public const string GET_CASHFLOWS = "get-cashflows";
            public const string GET_CASHFLOW = "get-cashflow";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<CashflowsController> _logger;

        public CashflowsController(IMediator mediator, ILogger<CashflowsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Routes.GET_UPCOMING, Name = Routes.GET_UPCOMING)]
        [ProducesResponseType(typeof(UpcomingViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetUpcoming([FromQuery] DateTime from, [FromQuery] DateTime to,
            CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetUpcomingQuery(from, to), cancellationToken);

            return Ok(response);
        }

        [HttpGet(Routes.GET_CASHFLOWS, Name = Routes.GET_CASHFLOWS)]
        [ProducesResponseType(typeof(CashflowViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCashflows([FromQuery] int? offset, int? limit, string[] sort,
            string[] filter, CancellationToken cancellationToken = default)
        {
            var (totalCount, cashflowViewModels) = await _mediator.Send(new GetCashflowsQuery(offset, limit, sort, filter),
                cancellationToken);

            Response?.Headers?.Add("X-Total-Count", $"{totalCount}");

            return Ok(cashflowViewModels);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_CASHFLOW)]
        [ProducesResponseType(typeof(CashflowViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCashflow(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetCashflowQuery {Id = id}, cancellationToken);

            return Ok(response);
        }
    }
}
