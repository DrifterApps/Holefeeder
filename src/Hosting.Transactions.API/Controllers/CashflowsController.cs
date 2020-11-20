﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using DrifterApps.Holefeeder.Application.Transactions.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Hosting.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace Hosting.Transactions.API.Controllers
{
    [Route("api/v2/[controller]"), Authorize(Policy = Policies.REGISTERED_USER)]
    public class CashflowsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_UPCOMING = "get_upcoming";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<CashflowsController> _logger;

        public CashflowsController(IMediator mediator, ILogger<CashflowsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet("upcoming", Name = Routes.GET_UPCOMING)]
        [ProducesResponseType(typeof(UpcomingViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] DateTime from, [FromQuery] DateTime to,
            CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            if (from > to)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new GetUpcomingQuery(userId, from, to), cancellationToken);

            return Ok(response);
        }
    }
}