#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Agibank.EventBus.Consumer/Agibank.EventBus.Consumer.csproj", "Agibank.EventBus.Consumer/"]
COPY ["Agibank.Shared/Agibank.Shared.csproj", "Agibank.Shared/"]
COPY ["Agibank.Domain/Agibank.Domain.csproj", "Agibank.Domain/"]
RUN dotnet restore "Agibank.EventBus.Consumer/Agibank.EventBus.Consumer.csproj"
COPY . .
WORKDIR "/src/Agibank.EventBus.Consumer"
RUN dotnet build "Agibank.EventBus.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Agibank.EventBus.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agibank.EventBus.Consumer.dll"]