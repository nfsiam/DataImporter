#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Workers/DataImporter.EmailWorker/DataImporter.EmailWorker.csproj", "Workers/DataImporter.EmailWorker/"]
COPY ["DataImporter.Core/DataImporter.Core.csproj", "DataImporter.Core/"]
COPY ["DataImporter.Common/DataImporter.Common.csproj", "DataImporter.Common/"]
COPY ["DataImporter.Membership/DataImporter.Membership.csproj", "DataImporter.Membership/"]
COPY ["DataImporter.Data/DataImporter.Data.csproj", "DataImporter.Data/"]
RUN dotnet restore "Workers/DataImporter.EmailWorker/DataImporter.EmailWorker.csproj"
COPY . .
WORKDIR "/src/Workers/DataImporter.EmailWorker"
RUN dotnet build "DataImporter.EmailWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataImporter.EmailWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataImporter.EmailWorker.dll"]