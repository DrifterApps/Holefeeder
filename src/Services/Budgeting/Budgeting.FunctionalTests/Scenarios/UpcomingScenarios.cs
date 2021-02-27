using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Extensions;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class UpcomingScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public UpcomingScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;

            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
        }

        [Scenario]
        public void GivenGetUpcoming_WithDateRange(DateTime from, DateTime to, HttpResponseMessage response)
        {
            "Given GetUpcoming query from 2020-01-15"
                .x(() => from = new DateTime(2020, 1, 15));

            "And for up to 2 weeks"
                .x(() => to = from.AddDays(14));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain a single cashflow"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(1)
                        .And.ContainEquivalentOf(
                            new UpcomingViewModel(BudgetingContextSeed.Cashflow1.Id,
                                new DateTime(2020, 1, 16), 111, "Cashflow1",
                                new CategoryInfoViewModel(BudgetingContextSeed.Category1.Id, "Category1",
                                    CategoryType.Expense, string.Empty),
                                new AccountInfoViewModel(BudgetingContextSeed.Account1.Id, "Account1",
                                    BudgetingContextSeed.Account1.MongoId.ToString()),
                                null));
                });
        }

        [Scenario]
        public void GivenGetUpcoming_WithUnpaidCashflows(DateTime from, DateTime to, HttpResponseMessage response,
            UpcomingViewModel[] result)
        {
            "Given GetUpcoming query from 2020-01-29"
                .x(() => from = new DateTime(2020, 1, 29));

            "And for up to 2 weeks"
                .x(() => to = from.AddDays(14));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());


            "And the result contain 2 cashflows"
                .x(async () =>
                {
                    result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(2);
                });

            "And be in ascending order on the Date"
                .x(() => result.Should().BeInAscendingOrder(x => x.Date));

            "With the unpaid cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel(BudgetingContextSeed.Cashflow1.Id,
                            new DateTime(2020, 1, 16), 111, "Cashflow1",
                            new CategoryInfoViewModel(BudgetingContextSeed.Category1.Id, "Category1",
                                CategoryType.Expense, string.Empty),
                            new AccountInfoViewModel(BudgetingContextSeed.Account1.Id, "Account1",
                                BudgetingContextSeed.Account1.MongoId.ToString()), null)));

            "And the current period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel(BudgetingContextSeed.Cashflow1.Id,
                            new DateTime(2020, 1, 30), 111, "Cashflow1",
                            new CategoryInfoViewModel(BudgetingContextSeed.Category1.Id, "Category1",
                                CategoryType.Expense, string.Empty),
                            new AccountInfoViewModel(BudgetingContextSeed.Account1.Id, "Account1",
                                BudgetingContextSeed.Account1.MongoId.ToString()), null)));
        }

        [Scenario]
        public void GivenGetUpcoming_ForMultiplePeriods(DateTime from, DateTime to, HttpResponseMessage response,
            UpcomingViewModel[] result)
        {
            "Given GetUpcoming query from 2020-01-15"
                .x(() => from = new DateTime(2020, 1, 15));

            "And for up to 4 weeks"
                .x(() => to = from.AddDays(28));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());


            "And the result contain 2 cashflows"
                .x(async () =>
                {
                    result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(2);
                });

            "And be in ascending order on the Date"
                .x(() => result.Should().BeInAscendingOrder(x => x.Date));

            "With the current period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel(BudgetingContextSeed.Cashflow1.Id,
                            new DateTime(2020, 1, 16), 111, "Cashflow1",
                            new CategoryInfoViewModel(BudgetingContextSeed.Category1.Id, "Category1",
                                CategoryType.Expense, string.Empty),
                            new AccountInfoViewModel(BudgetingContextSeed.Account1.Id, "Account1",
                                BudgetingContextSeed.Account1.MongoId.ToString()), null)));

            "And the following period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel(BudgetingContextSeed.Cashflow1.Id,
                            new DateTime(2020, 1, 30), 111, "Cashflow1",
                            new CategoryInfoViewModel(BudgetingContextSeed.Category1.Id, "Category1",
                                CategoryType.Expense, string.Empty),
                            new AccountInfoViewModel(BudgetingContextSeed.Account1.Id, "Account1",
                                BudgetingContextSeed.Account1.MongoId.ToString()), null)));
        }
    }
}
