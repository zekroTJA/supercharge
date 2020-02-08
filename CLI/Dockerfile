FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CLI/CLI.csproj", "CLI/"]
COPY ["DatabaseAccessLayer/DatabaseAccessLayer.csproj", "DatabaseAccessLayer/"]
COPY ["RiotAPIAccessLayer/RiotAPIAccessLayer.csproj", "RiotAPIAccessLayer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "CLI/CLI.csproj"
COPY . .
WORKDIR "/src/CLI"
RUN dotnet build "CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CLI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CLI.dll"]
CMD ["--help"]