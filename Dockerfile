# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Solo src: la solución incluye tests/ y en imagen de runtime no hace falta copiarlos.
COPY src ./src

RUN dotnet restore src/RickAndMorty.Api/RickAndMorty.Api.csproj
RUN dotnet publish src/RickAndMorty.Api/RickAndMorty.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RickAndMorty.Api.dll"]
