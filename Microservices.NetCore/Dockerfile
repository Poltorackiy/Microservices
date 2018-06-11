FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Microservices.NetCore/Microservices.NetCore.csproj Microservices.NetCore/
RUN dotnet restore Microservices.NetCore/Microservices.NetCore.csproj
COPY . .
WORKDIR /src/Microservices.NetCore
RUN dotnet build Microservices.NetCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Microservices.NetCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Microservices.NetCore.dll"]
