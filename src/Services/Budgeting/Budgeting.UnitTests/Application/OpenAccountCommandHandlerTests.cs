﻿using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;

using FluentValidation;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class OpenAccountCommandHandlerTests
    {
        [Fact]
        public void GivenOpenAccountCommand_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            // arrange
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            Func<Task> action = async () => await handler.Handle(null, CancellationToken.None);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenOpenAccountCommand_WhenRequestIsValid_ThenReturnCommandCreatedWithId()
        {
            // arrange
            var command = new OpenAccountCommand {Name = "New account", OpenDate = DateTime.Today};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(CommandResult<Guid>.Create(CommandStatus.Created, Guid.NewGuid()),
                options => options
                    .ComparingByMembers<CommandResult<Guid>>()
                    .Using<Guid>(ctx => ctx.Subject.Should().NotBeEmpty()).WhenTypeIs<Guid>());
        }

        [Fact]
        public async Task GivenOpenAccountCommand_WhenRequestIsMissingName_ThenReturnCommandBadRequest()
        {
            // arrange
            var command = new OpenAccountCommand {OpenDate = DateTime.Today};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(
                CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    "name must be from 1 to 255 characters"),
                options => options.ComparingByMembers<CommandResult<Guid>>());
        }

        [Fact]
        public async Task GivenOpenAccountCommand_WhenRequestHasNameTooLong_ThenReturnCommandBadRequest()
        {
            // arrange
            const string longString
                = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*(){}|[]\\;':\",./<>?";
            var command = new OpenAccountCommand
            {
                Name = string.Concat(longString, longString, longString), OpenDate = DateTime.Today
            };
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(
                CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    "name must be from 1 to 255 characters"),
                options => options.ComparingByMembers<CommandResult<Guid>>());
        }

        [Fact]
        public async Task GivenOpenAccountCommand_WhenRequestIsMissingOpenDate_ThenReturnCommandBadRequest()
        {
            // arrange
            var command = new OpenAccountCommand {Name = "new account"};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(
                CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    "openDate is required"),
                options => options.ComparingByMembers<CommandResult<Guid>>());
        }

        [Fact]
        public async Task GivenOpenAccountCommand_WhenRequestIsMissingUserId_ThenReturnCommandBadRequest()
        {
            // arrange
            var command = new OpenAccountCommand {Name = "new account", OpenDate = DateTime.Today};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.Empty;
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(
                CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    "userId is required"),
                options => options.ComparingByMembers<CommandResult<Guid>>());
        }

        [Fact]
        public void GivenOpenAccountCommand_WhenAccountNameAlreadyExists_ThenReturnCommandConflict()
        {
            // arrange
            var command = new OpenAccountCommand {Name = "new account", OpenDate = DateTime.Today};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(new Account(Guid.NewGuid(), AccountType.Checking, "new account", false, Decimal.One,
                    DateTime.Today, "description", false, Guid.NewGuid()));
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<OpenAccountCommandHandler>>();
            var handler = new OpenAccountCommandHandler(repository, cache, logger);

            // act
            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            // assert
            action.Should().Throw<ValidationException>().WithMessage("Account name 'new account' already exists");
        }
    }
}
