#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Identity_Server/Identity_Server.csproj", "src/Identity_Server/"]
RUN dotnet restore "src/Identity_Server/Identity_Server.csproj"
COPY . .
WORKDIR "/src/src/Identity_Server"
RUN dotnet build "Identity_Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity_Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity_Server.dll"]