using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Converters;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class AccountScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public AccountScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;

            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            _jsonSerializerOptions.Converters.Add(new AccountTypeConverter());
            _jsonSerializerOptions.Converters.Add(new CategoryTypeConverter());
        }

        [Scenario]
        public void GivenGetAccounts(HttpClient client,
            HttpResponseMessage response)
        {
            "Given GetAccount query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/accounts/get-accounts";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the accounts of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(3)
                        .And.BeEquivalentTo(
                            new AccountViewModel(BudgetingContextSeed.Account1.Id, AccountType.Checking, "Account1", 6,
                                19.01m, new DateTime(2020, 1, 7), "Description1", false),
                            new AccountViewModel(BudgetingContextSeed.Account2.Id, AccountType.CreditCard, "Account2",
                                0, 200.02m, new DateTime(2019, 1, 3), "Description2", true),
                            new AccountViewModel(BudgetingContextSeed.Account3.Id, AccountType.Loan, "Account3", 0,
                                300.03m, new DateTime(2019, 1, 4), "Description3", false));
                });
        }

        [Scenario]
        public void GivenGetAccount_WhenWithValidId_ThenReturnResultForUser(HttpClient client, Guid accountId,
            HttpResponseMessage response)
        {
            "Given getting account by id #2"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    accountId = BudgetingContextSeed.Account2.Id;
                });

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid1.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    string request = $"/api/v2/accounts/{accountId.ToString()}";

                    // Act
                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the accounts of the user"
                .x(async () =>
                {
                    var jsonOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                    jsonOptions.Converters.Add(new AccountTypeConverter());
                    jsonOptions.Converters.Add(new CategoryTypeConverter());
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel>(jsonOptions);

                    result.Should().BeEquivalentTo(
                        new AccountViewModel(BudgetingContextSeed.Account2.Id, AccountType.CreditCard, "Account2", 0,
                            200.02m, new DateTime(2019, 1, 3), "Description2", true)
                    );
                });
        }
    }
}
