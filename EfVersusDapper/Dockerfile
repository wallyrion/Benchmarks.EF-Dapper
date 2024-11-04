# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
ARG TARGETARCH

# Copy project file and restore as distinct layers
# Copy source code and publish app
COPY --link EfVersusDapper.csproj .
RUN dotnet restore -r linux-amd64

COPY --link . .

RUN dotnet publish --no-restore -r linux-amd64 -o /app EfVersusDapper.csproj


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get -y update; apt-get -y install curl


EXPOSE 8080
WORKDIR /app
COPY --link --from=build /app .

USER $APP_UID

HEALTHCHECK CMD curl --fail http://localhost:8080/healthz || exit

ENTRYPOINT ["./EfVersusDapper"]