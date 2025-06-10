FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY FormFlow.sln .
COPY FormFlow.Domain/FormFlow.Domain.csproj FormFlow.Domain/
COPY FormFlow.Application/FormFlow.Application.csproj FormFlow.Application/
COPY FormFlow.Persistence/FormFlow.Persistence.csproj FormFlow.Persistence/
COPY FormFlow.Infrastructure/FormFlow.Infrastructure.csproj FormFlow.Infrastructure/
COPY FormFlow.WebApi/FormFlow.WebApi.csproj FormFlow.WebApi/
RUN dotnet restore FormFlow.WebApi/FormFlow.WebApi.csproj

COPY . .
RUN dotnet build FormFlow.WebApi/FormFlow.WebApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FormFlow.WebApi/FormFlow.WebApi.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FormFlow.WebApi.dll"]