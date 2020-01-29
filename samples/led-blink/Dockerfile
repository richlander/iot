FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# copy and build app
COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./led-blink"]
