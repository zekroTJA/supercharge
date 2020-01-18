FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Crawler/Crawler.csproj", "Crawler/"]
COPY ["DatabaseAccessLayer/DatabaseAccessLayer.csproj", "DatabaseAccessLayer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["RiotAPIAccessLayer/RiotAPIAccessLayer.csproj", "RiotAPIAccessLayer/"]
RUN dotnet restore "Crawler/Crawler.csproj"
COPY . .
WORKDIR "/src/Crawler"
RUN dotnet build "Crawler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crawler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crawler.dll"]