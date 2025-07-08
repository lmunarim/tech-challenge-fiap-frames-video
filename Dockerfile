# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
#EXPOSE 443
EXPOSE 1433

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["/src/Presentation/fiap.API/fiap.API.csproj", "src/Presentation/fiap.API/"]
COPY ["/src/Presentation/fiap.API/fiap.API/lib", "src/Presentation/fiap.API/lib"]
COPY ["/src/Presentation/fiap.API/fiap.API/outputs", "src/Presentation/fiap.API/outputs"]
COPY ["/src/Presentation/fiap.API/fiap.API/temporary", "src/Presentation/fiap.API/temporary"]

RUN dotnet restore "./src/Presentation/fiap.API/fiap.API.csproj"
COPY . .
WORKDIR "/src/Presentation/fiap.API"
RUN dotnet build "./fiap.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./fiap.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN dotnet tool install -g Microsoft.dotnet-httprepl
RUN dotnet dev-certs https --trust
# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "fiap.API.dll"]