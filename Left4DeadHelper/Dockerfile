#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR "/src"
COPY ["Left4DeadHelper/Left4DeadHelper.csproj", "Left4DeadHelper/"]
COPY ["Left4DeadHelper.Sprays/Left4DeadHelper.Sprays.csproj", "Left4DeadHelper.Sprays/"]
COPY ["Left4DeadHelper.ImageSharpExtensions/Left4DeadHelper.ImageSharpExtensions.csproj", "Left4DeadHelper.ImageSharpExtensions/"]
COPY ["Left4DeadHelper.Tests.Unit/Left4DeadHelper.Tests.Unit.csproj", "Left4DeadHelper.Tests.Unit/"]
RUN dotnet restore "Left4DeadHelper/Left4DeadHelper.csproj"
RUN dotnet restore "Left4DeadHelper.Tests.Unit/Left4DeadHelper.Tests.Unit.csproj"
COPY . .
WORKDIR "/src/Left4DeadHelper"
RUN dotnet build "Left4DeadHelper.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR "/src/Left4DeadHelper.Tests.Unit"
RUN dotnet test -c Release "Left4DeadHelper.Tests.Unit.csproj"

FROM build AS publish
WORKDIR "/src/Left4DeadHelper"
RUN dotnet publish "Left4DeadHelper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Left4DeadHelper.dll"]
