FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/GastosControl.Domain/GastosControl.Domain.csproj backend/src/GastosControl.Domain/
COPY backend/src/GastosControl.Application/GastosControl.Application.csproj backend/src/GastosControl.Application/
COPY backend/src/GastosControl.Infrastructure/GastosControl.Infrastructure.csproj backend/src/GastosControl.Infrastructure/
COPY backend/src/GastosControl.Api/GastosControl.Api.csproj backend/src/GastosControl.Api/

RUN dotnet restore backend/src/GastosControl.Api/GastosControl.Api.csproj

COPY backend/src backend/src
RUN dotnet publish backend/src/GastosControl.Api/GastosControl.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GastosControl.Api.dll"]
