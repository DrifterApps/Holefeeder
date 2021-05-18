using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class GetUpcomingHandlerTests
    {
        [Fact]
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var handler = new GetUpcomingHandler(Substitute.For<IUpcomingQueriesRepository>(), Substitute.For<ItemsCache>());

            Func<Task> action = async () => await handler.Handle(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<IUpcomingQueriesRepository>();
            repository.GetUpcomingAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(),
                    CancellationToken.None)
                .Returns(new List<UpcomingViewModel>());

            var handler = new GetUpcomingHandler(repository, cache);


            Func<Task> action = async () =>
                await handler.Handle(new GetUpcomingQuery(DateTime.Today, DateTime.Today.AddDays(14)));

            await action.Should().NotThrowAsync();
        }
    }
}
