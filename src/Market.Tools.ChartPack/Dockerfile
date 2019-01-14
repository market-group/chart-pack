FROM microsoft/dotnet:2.2-sdk AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Market.Tools.ChartPack/Market.Tools.ChartPack.csproj src/Market.Tools.ChartPack/
RUN dotnet restore src/Market.Tools.ChartPack/Market.Tools.ChartPack.csproj
COPY . .
WORKDIR /src/src/Market.Tools.ChartPack
RUN dotnet build Market.Tools.ChartPack.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Market.Tools.ChartPack.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Market.Tools.ChartPack.dll"]
