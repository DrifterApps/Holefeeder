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
    public class CategoriesController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_CATEGORIES = "get-categories";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Routes.GET_CATEGORIES, Name = Routes.GET_CATEGORIES)]
        [ProducesResponseType(typeof(CategoryViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);

            return Ok(response);
        }
    }
}
