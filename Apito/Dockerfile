# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
# Set non-interactive mode for APT
ENV DEBIAN_FRONTEND=noninteractive 
# Fix for package update issues
RUN apt-get clean && apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    libgdiplus \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libfontconfig1 \
    fontconfig \
    libfreetype6 \
    libjpeg62-turbo \
    libpng16-16 \
    libexpat1 \
    libuuid1 \
    libxcb1 \
    && rm -rf /var/lib/apt/lists/*
# Install wkhtmltopdf (static binary version)
RUN curl -SL https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox_0.12.6-1.bookworm_amd64.deb -o wkhtmltopdf.deb \
    && dpkg -i wkhtmltopdf.deb \
    && apt-get -f install -y \
    && rm wkhtmltopdf.deb
EXPOSE 80
EXPOSE 8081
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apito/Apito.csproj", "Apito/"]
RUN dotnet restore "./Apito/Apito.csproj"
COPY . .
WORKDIR "/src/Apito"
RUN dotnet build "./Apito.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Apito.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apito.dll"]