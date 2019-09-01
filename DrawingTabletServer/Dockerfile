FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["DrawingTabletServer/DrawingTabletServer.csproj", "DrawingTabletServer/"]
COPY ["DrawingTablet.Core/DrawingTablet.Core.csproj", "DrawingTablet.Core/"]
RUN dotnet restore "DrawingTabletServer/DrawingTabletServer.csproj"
COPY . .
WORKDIR "/src/DrawingTabletServer"
RUN dotnet build "DrawingTabletServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DrawingTabletServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DrawingTabletServer.dll"]