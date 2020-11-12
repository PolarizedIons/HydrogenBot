FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./HydrogenBot/HydrogenBot.csproj .
RUN dotnet restore

COPY ./HydrogenBot .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "HydrogenBot.dll"]
