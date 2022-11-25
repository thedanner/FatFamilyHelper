#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR "/src"
COPY ["FatFamilyHelper/FatFamilyHelper.csproj", "FatFamilyHelper/"]
COPY ["FatFamilyHelper.Sprays/FatFamilyHelper.Sprays.csproj", "FatFamilyHelper.Sprays/"]
COPY ["FatFamilyHelper.ImageSharpExtensions/FatFamilyHelper.ImageSharpExtensions.csproj", "FatFamilyHelper.ImageSharpExtensions/"]
COPY ["FatFamilyHelper.Tests.Unit/FatFamilyHelper.Tests.Unit.csproj", "FatFamilyHelper.Tests.Unit/"]
RUN dotnet restore "FatFamilyHelper/FatFamilyHelper.csproj"
RUN dotnet restore "FatFamilyHelper.Tests.Unit/FatFamilyHelper.Tests.Unit.csproj"
COPY . .
WORKDIR "/src/FatFamilyHelper"
RUN dotnet build "FatFamilyHelper.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR "/src/FatFamilyHelper.Tests.Unit"
RUN dotnet test -c Release "FatFamilyHelper.Tests.Unit.csproj"

FROM build AS publish
WORKDIR "/src/FatFamilyHelper"
RUN dotnet publish "FatFamilyHelper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FatFamilyHelper.dll"]
