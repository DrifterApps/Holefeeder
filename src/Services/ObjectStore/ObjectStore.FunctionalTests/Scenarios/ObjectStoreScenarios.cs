using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.API;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

using Xbehave;

using Xunit;

namespace ObjectStore.FunctionalTests.Scenarios
{
    public class ObjectStoreScenarios : IClassFixture<ObjectStoreWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true,};

        public ObjectStoreScenarios(ObjectStoreWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Scenario]
        public void GetStoreItems(HttpClient client, HttpResponseMessage response)
        {
            "Given GetStoreItems query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/StoreItems/get-store-items";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the store items of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(3)
                        .And.Contain(new[]
                        {
                            new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"),
                            new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"),
                            new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code3", "Data3")
                        });
                });
        }

        [Scenario]
        public void GetStoreItems_WithFilter(HttpClient client, string filter, HttpResponseMessage response)
        {
            "Given GetStoreItems query"
                .x(() => client = _factory.CreateDefaultClient());

            "With filter code=Code2"
                .x(() => filter = "code=Code2");

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/get-store-items?filter={filter}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(1)
                        .And.Contain(new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"));
                });
        }

        [Scenario]
        public void GetStoreItems_WithQueryParams(HttpClient client, string sort, int offset, int limit,
            HttpResponseMessage response)
        {
            "Given GetStoreItems query"
                .x(() => client = _factory.CreateDefaultClient());

            "Sorted on code descending"
                .x(() => sort = "-code");

            "With an offset of 1"
                .x(() => offset = 1);

            "And a limit of 2"
                .x(() => limit = 2);

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/get-store-items?sort={sort}&offset={offset}&limit={limit}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(2)
                        .And.ContainInOrder(
                            new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"),
                            new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"));
                });
        }

        [Scenario]
        public void GetStoreItem(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given GetStoreItem query"
                .x(() => client = _factory.CreateDefaultClient());

            "With Id #3"
                .x(() => id = StoreItemContextSeed.Guid3);

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeNull()
                        .And.Be(new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code3", "Data3"));
                });
        }

        [Scenario]
        public void GetStoreItem_WithInvalidId(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given GetStoreItem query"
                .x(() => client = _factory.CreateDefaultClient());

            "With an invalid Id"
                .x(() => id = Guid.Empty);

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate BadRequest"
                .x(() => response.StatusCode.Should().Be(StatusCodes.Status400BadRequest));
        }

        [Scenario]
        public void GetStoreItem_WhenIdDoesntExists(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given GetStoreItem query"
                .x(() => client = _factory.CreateDefaultClient());

            "With non existent Id"
                .x(() => id = Guid.NewGuid());

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate NotFound"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound));
        }

        [Scenario]
        public void GetStoreItem_WithIdFromAnotherUser(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given GetStoreItem query"
                .x(() => client = _factory.CreateDefaultClient());

            "With item Id belonging to TestUser #2"
                .x(() => id = StoreItemContextSeed.Guid4);

            "When I call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate NotFound"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound));
        }

        [Scenario]
        public void CreateStoreItemCommand(HttpClient client, CreateStoreItemCommand command,
            HttpResponseMessage response)
        {
            "Given CreateStoreItem command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "With valid data"
                .x(() => command = new CreateStoreItemCommand {Code = "New Code", Data = "New Data"});

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/StoreItems/create-store-item";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "With the header location present"
                .x(() => response.Headers.Location?.AbsolutePath.Should()
                    .BeEquivalentTo("/api/v2/StoreItems/create-store-item"));

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

        [Scenario]
        public void CreateObjectCommand_WithInvalidCommand(HttpClient client, HttpResponseMessage response)
        {
            "Given CreateStoreItem command"
                .x(() => client = _factory.CreateDefaultClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "When I call the API with invalid data"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/StoreItems/create-store-item";

                    response = await client.PostAsJsonAsync(requestUri, new { });
                });

            "Then the status code should indicate BadRequest"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest));
        }

        [Scenario]
        public void CreateStoreItemCommand_WhenCodeAlreadyExist(HttpClient client, CreateStoreItemCommand command,
            HttpResponseMessage response)
        {
            "Given CreateStoreItem command"
                .x(() => client = _factory.CreateClient());

            "With valid data"
                .x(() => command = new CreateStoreItemCommand {Code = "Code1", Data = "Data1"});

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/StoreItems/create-store-item";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate BadRequest"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest));

            "And a CommandResult with created status"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);
                    result.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                                "Code 'Code1' already exists."),
                            options => options.ComparingByMembers<CommandResult<Guid>>());
                });
        }

        [Scenario]
        public void ModifyStoreItemCommand(HttpClient client, Guid data, CommandResult<Guid> createResult,
            ModifyStoreItemCommand modifyCommand, CommandResult modifyResult,
            HttpResponseMessage modifyResponse)
        {
            "Given ModifyStoreItem command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "On existing store item"
                .x(async () =>
                {
                    const string createRequestUri = "/api/v2/StoreItems/create-store-item";

                    var code = Guid.NewGuid();
                    data = Guid.NewGuid();

                    var createCommand = new CreateStoreItemCommand {Code = code.ToString(), Data = data.ToString()};
                    var createResponse = await client.PostAsJsonAsync(createRequestUri, createCommand);

                    createResult =
                        await createResponse.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);
                });

            "And modifying the data property"
                .x(() => modifyCommand =
                    new ModifyStoreItemCommand
                    {
                        Id = createResult?.Result ?? Guid.Empty, Data = $"{data.ToString()}-modified"
                    });

            "When I call the API"
                .x(async () =>
                {
                    const string modifyRequestUri = "/api/v2/StoreItems/modify-store-item";

                    modifyResponse = await client.PutAsJsonAsync(modifyRequestUri, modifyCommand);
                });

            "Then the status code should indicate Success"
                .x(() => modifyResponse.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And a CommandResult with ok status"
                .x(async () =>
                {
                    var result = await modifyResponse.Content.ReadFromJsonAsync<CommandResult>(_jsonSerializerOptions);
                    result.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo(CommandResult.Create(CommandStatus.Ok),
                            options => options.ComparingByMembers<CommandResult>());
                });
        }
    }
}
