FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HrDirectorRabbitService/HrDirectorRabbitService.csproj", "HrDirectorRabbitService/"]
RUN dotnet restore "HrDirectorRabbitService/HrDirectorRabbitService.csproj"

COPY . .
WORKDIR "/src/HrDirectorRabbitService"
RUN dotnet build "HrDirectorRabbitService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HrDirectorRabbitService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HrDirectorRabbitService.dll"]