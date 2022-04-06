FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Memecraft/Memecraft.csproj", "Memecraft/"]
RUN dotnet restore "Memecraft/Memecraft.csproj"
COPY . .
WORKDIR "/src/Memecraft"
RUN dotnet build "Memecraft.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Memecraft.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Memecraft.dll"]
