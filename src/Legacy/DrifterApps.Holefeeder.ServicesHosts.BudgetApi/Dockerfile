FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY DrifterApps.Holefeeder.sln ./
COPY Directory.Build.props ./

COPY src/ApiGateways/Web.Gateway/*.csproj src/ApiGateways/Web.Gateway/
COPY src/BuildingBlocks/Framework.Mongo.SeedWork/*.csproj src/BuildingBlocks/Framework.Mongo.SeedWork/
COPY src/BuildingBlocks/Framework.SeedWork/*.csproj src/BuildingBlocks/Framework.SeedWork/
COPY src/Services/Budgeting/Budgeting.API/*.csproj src/Services/Budgeting/Budgeting.API/
COPY src/Services/Budgeting/Budgeting.Application/*.csproj src/Services/Budgeting/Budgeting.Application/
COPY src/Services/Budgeting/Budgeting.Domain/*.csproj src/Services/Budgeting/Budgeting.Domain/
COPY src/Services/Budgeting/Budgeting.Infrastructure/*.csproj src/Services/Budgeting/Budgeting.Infrastructure/
COPY src/Services/ObjectStore/ObjectStore.API/*.csproj src/Services/ObjectStore/ObjectStore.API/
COPY src/Services/ObjectStore/ObjectStore.Application/*.csproj src/Services/ObjectStore/ObjectStore.Application/
COPY src/Services/ObjectStore/ObjectStore.Domain/*.csproj src/Services/ObjectStore/ObjectStore.Domain/
COPY src/Services/ObjectStore/ObjectStore.Infrastructure/*.csproj src/Services/ObjectStore/ObjectStore.Infrastructure/

COPY src/Services/Budgeting/Budgeting.UnitTests/*.csproj src/Services/Budgeting/Budgeting.UnitTests/
COPY src/Services/Budgeting/Budgeting.FunctionalTests/*.csproj src/Services/Budgeting/Budgeting.FunctionalTests/
COPY src/Services/ObjectStore/ObjectStore.UnitTests/*.csproj src/Services/ObjectStore/ObjectStore.UnitTests/
COPY src/Services/ObjectStore/ObjectStore.FunctionalTests/*.csproj src/Services/ObjectStore/ObjectStore.FunctionalTests/

COPY src/Legacy/DrifterApps.Holefeeder.Business/*.csproj src/Legacy/DrifterApps.Holefeeder.Business/
COPY src/Legacy/DrifterApps.Holefeeder.Business.Contracts/*.csproj src/Legacy/DrifterApps.Holefeeder.Business.Contracts/
COPY src/Legacy/DrifterApps.Holefeeder.Business.Entities/*.csproj src/Legacy/DrifterApps.Holefeeder.Business.Entities/
COPY src/Legacy/DrifterApps.Holefeeder.Common/*.csproj src/Legacy/DrifterApps.Holefeeder.Common/
COPY src/Legacy/DrifterApps.Holefeeder.Common.IoC/*.csproj src/Legacy/DrifterApps.Holefeeder.Common.IoC/
COPY src/Legacy/DrifterApps.Holefeeder.ResourcesAccess.Contracts/*.csproj src/Legacy/DrifterApps.Holefeeder.ResourcesAccess.Contracts/
COPY src/Legacy/DrifterApps.Holefeeder.ResourcesAccess.Mongo/*.csproj src/Legacy/DrifterApps.Holefeeder.ResourcesAccess.Mongo/
COPY src/Legacy/DrifterApps.Holefeeder.Services.BudgetApi/*.csproj src/Legacy/DrifterApps.Holefeeder.Services.BudgetApi/
COPY src/Legacy/DrifterApps.Holefeeder.Services.BudgetDto/*.csproj src/Legacy/DrifterApps.Holefeeder.Services.BudgetDto/
COPY src/Legacy/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/*.csproj src/Legacy/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/

RUN dotnet restore DrifterApps.Holefeeder.sln

FROM restore as build
ARG holefeeder_version=99.99.99
WORKDIR /src
COPY src/. src/.
WORKDIR /src/src/Legacy/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/
RUN dotnet publish /property:Version=${holefeeder_version} --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DrifterApps.Holefeeder.ServicesHosts.BudgetApi.dll"]
