FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pawesome/Pawesome.csproj", "Pawesome/"]
COPY ["Pawesome.Tests/Pawesome.Tests.csproj", "Pawesome.Tests/"]
RUN dotnet restore "Pawesome/Pawesome.csproj"

COPY . .
WORKDIR "/src/Pawesome"

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN libman restore

RUN dotnet build "Pawesome.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pawesome.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pawesome.dll"]