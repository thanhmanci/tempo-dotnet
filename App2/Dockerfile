#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/sdk:6.0.301-bullseye-slim-amd64 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.301-bullseye-slim-amd64 AS build
WORKDIR /src
COPY ["App2/App2.csproj", "App2/"]
RUN dotnet restore "App2/App2.csproj"
COPY . .
WORKDIR "/src/App2"
RUN dotnet build "App2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App2.dll"]