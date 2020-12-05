FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["RestAPI/RestAPI.csproj", "RestAPI/"]
COPY ["DatabaseAccessLayer/DatabaseAccessLayer.csproj", "DatabaseAccessLayer/"]
COPY ["DDragonAccessLayer/DDragonAccessLayer.csproj", "DDragonAccessLayer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["RiotAPIAccessLayer/RiotAPIAccessLayer.csproj", "RiotAPIAccessLayer/"]
RUN dotnet restore "RestAPI/RestAPI.csproj"
COPY . .
WORKDIR "/src/RestAPI"
RUN dotnet build "RestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestAPI.dll"]
