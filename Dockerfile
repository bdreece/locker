# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /usr/share/locker/

COPY ./Locker.csproj .
RUN dotnet restore

COPY ./* ./
RUN dotnet publish -c Release --no-restore

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS runtime
WORKDIR /usr/share/locker/bin

COPY --from=build /usr/share/locker/bin/Release/net7.0/publish/ ./
ENTRYPOINT ["dotnet", "Locker.dll"]