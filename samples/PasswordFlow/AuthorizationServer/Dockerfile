FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AuthorizationServer.csproj", ""]
RUN dotnet restore "AuthorizationServer.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "AuthorizationServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AuthorizationServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthorizationServer.dll"]