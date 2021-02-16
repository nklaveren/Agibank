#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AnaliseDados.EventBus.Producer/AnaliseDados.EventBus.Producer.csproj", "AnaliseDados.EventBus.Producer/"]
COPY ["AnaliseDados.Domain/AnaliseDados.Domain.csproj", "AnaliseDados.Domain/"]
RUN dotnet restore "AnaliseDados.EventBus.Producer/AnaliseDados.EventBus.Producer.csproj"
COPY . .
WORKDIR "/src/AnaliseDados.EventBus.Producer"
RUN dotnet build "AnaliseDados.EventBus.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnaliseDados.EventBus.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnaliseDados.EventBus.Producer.dll"]