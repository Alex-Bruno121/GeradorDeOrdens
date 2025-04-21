# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderAccumulator.Api/OrderAccumulator.Api/OrderAccumulator.Api.csproj", "OrderAccumulator.Api/OrderAccumulator.Api/"]
RUN dotnet restore "OrderAccumulator.Api/OrderAccumulator.Api/OrderAccumulator.Api.csproj"
COPY . .
WORKDIR "/src/OrderAccumulator.Api/OrderAccumulator.Api"
RUN dotnet build "OrderAccumulator.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderAccumulator.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderAccumulator.Api.dll"]
