#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DataImporter.Web/DataImporter.Web.csproj", "DataImporter.Web/"]
COPY ["DataImporter.Core/DataImporter.Core.csproj", "DataImporter.Core/"]
COPY ["DataImporter.Common/DataImporter.Common.csproj", "DataImporter.Common/"]
COPY ["DataImporter.Membership/DataImporter.Membership.csproj", "DataImporter.Membership/"]
COPY ["DataImporter.Data/DataImporter.Data.csproj", "DataImporter.Data/"]
RUN dotnet restore "DataImporter.Web/DataImporter.Web.csproj"
COPY . .
WORKDIR "/src/DataImporter.Web"
RUN dotnet build "DataImporter.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataImporter.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataImporter.Web.dll"]
