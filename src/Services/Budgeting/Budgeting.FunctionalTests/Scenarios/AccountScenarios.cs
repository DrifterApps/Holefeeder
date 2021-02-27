using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Converters;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;
using FluentAssertions.Execution;

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
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new AccountViewModel(BudgetingContextSeed.Account2.Id, AccountType.CreditCard, "Account2", 0,
                            200.02m, new DateTime(2019, 1, 3), "Description2", true)
                    );
                });
        }

        [Scenario]
        public void OpenAccountCommand(HttpClient client, OpenAccountCommand command,
            HttpResponseMessage response)
        {
            "Given OpenAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "With valid data"
                .x(() => command = new OpenAccountCommand
                {
                    Type = AccountType.Checking,
                    Name = "New Account",
                    OpenBalance = 1234m,
                    OpenDate = DateTime.Today,
                    Description = "New account description"
                });

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/open-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "With the header location present"
                .x(() => response.Headers.Location?.AbsolutePath.Should().StartWithEquivalent("/api/v2/accounts/"));

            "And a CommandResult with created status"
                .x(async () =>
                {
                    using (new AssertionScope())
                    {
                        var result =
                            await response.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);
                        result.Should().NotBeNull()
                            .And.BeEquivalentTo(CommandResult<Guid>.Create(CommandStatus.Created, Guid.Empty),
                                options => options
                                    .ComparingByMembers<CommandResult<Guid>>()
                                    .Using<Guid>(ctx => ctx.Subject.Should().NotBeEmpty()).WhenTypeIs<Guid>());
                    }
                });
        }
    }
}