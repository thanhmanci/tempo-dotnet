#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["App3/App3.csproj", "App3/"]
RUN dotnet restore "App3/App3.csproj"
COPY . .
WORKDIR "/src/App3"
RUN dotnet build "App3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App3.csproj" -c Release -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Development
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App3.dll"]