#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Citrix.Unified.Api.Test.WebClient/Citrix.Unified.Api.Test.WebClient.csproj", "Citrix.Unified.Api.Test.WebClient/"]
RUN dotnet restore "Citrix.Unified.Api.Test.WebClient/Citrix.Unified.Api.Test.WebClient.csproj"
COPY . .
WORKDIR "/src/Citrix.Unified.Api.Test.WebClient"
RUN dotnet build "Citrix.Unified.Api.Test.WebClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Citrix.Unified.Api.Test.WebClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Citrix.Unified.Api.Test.WebClient.dll"]