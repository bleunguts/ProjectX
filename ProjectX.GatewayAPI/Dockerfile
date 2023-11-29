#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG APP_BUILD=/app/build
WORKDIR /src
RUN ls .
COPY ["ProjectX.GatewayAPI/ProjectX.GatewayAPI.csproj", "ProjectX.GatewayAPI/"]
COPY ["ProjectX.AnalyticsLibNativeShim/ProjectX.AnalyticsLibNativeShim.csproj", "ProjectX.AnalyticsLibNativeShim/"]
COPY ["ProjectX.AnalyticsLib.Shared/ProjectX.AnalyticsLib.Shared.csproj", "ProjectX.AnalyticsLib.Shared/"]
COPY ["ProjectX.Core/ProjectX.Core.csproj", "ProjectX.Core/"]
COPY ["ProjectX.AnalyticsLib/ProjectX.AnalyticsLib.csproj", "ProjectX.AnalyticsLib/"]
COPY ["ProjectX.MarketData/ProjectX.MarketData.csproj", "ProjectX.MarketData/"]
COPY ["ProjectX.AnalyticsLibNative/.", "ProjectX.AnalyticsLibNative/"]
RUN dotnet restore "./ProjectX.GatewayAPI/./ProjectX.GatewayAPI.csproj"
COPY . .
# native dependencies
RUN apt-get update && apt-get -y install cmake
RUN apt-get update && apt-get -y install build-essential
RUN rm -rf ./ProjectX.AnalyticsLibNative/out && rm -f ./ProjectX.AnalyticsLibNative/out/CMakeCache.txt
RUN cmake -S ./ProjectX.AnalyticsLibNative -B ./ProjectX.AnalyticsLibNative/out -DCMAKE_INSTALL_PREFIX=./ProjectX.AnalyticsLibNative/out/install/linux-base
RUN cmake --build ./ProjectX.AnalyticsLibNative/out  
RUN ls ./ProjectX.AnalyticsLibNative/out/bin
RUN mkdir -p $APP_BUILD/3rdParty
RUN cp ./ProjectX.AnalyticsLibNative/out/bin/ProjectX.AnalyticsLibNative.so $APP_BUILD/3rdParty
RUN ls $APP_BUILD/3rdParty
# main csproj
WORKDIR /src/ProjectX.GatewayAPI
RUN dotnet build "./ProjectX.GatewayAPI.csproj" -c $BUILD_CONFIGURATION -o $APP_BUILD

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectX.GatewayAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG SERVER_PORT 8080
ENV ASPNETCORE_HTTP_PORTS $SERVER_PORT
ENV ASPNETCORE_URLS http://localhost:$SERVER_PORT
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectX.GatewayAPI.dll"]