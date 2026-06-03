# syntax=docker/dockerfile:1

# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (cache layer): copy only project files + shared build props
COPY Directory.Build.props ./
COPY Futelo.Shared/Futelo.Shared.csproj Futelo.Shared/
COPY Futelo.Client/Futelo.Client.csproj Futelo.Client/
COPY Futelo.Server/Futelo.Server.csproj Futelo.Server/
RUN dotnet restore Futelo.Server/Futelo.Server.csproj

# Copy the rest and publish (Server pulls in the Client WASM as static assets)
COPY Futelo.Shared/ Futelo.Shared/
COPY Futelo.Client/ Futelo.Client/
COPY Futelo.Server/ Futelo.Server/
RUN dotnet publish Futelo.Server/Futelo.Server.csproj -c Release -o /app /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Railway injects $PORT at runtime; bind Kestrel to it (fallback 8080 for local runs)
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet Futelo.Server.dll"]
