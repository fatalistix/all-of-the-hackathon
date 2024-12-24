FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HrManagerRabbitService/HrManagerRabbitService.csproj", "HrManagerRabbitService/"]
RUN dotnet restore "HrManagerRabbitService/HrManagerRabbitService.csproj"

COPY . .
WORKDIR "/src/HrManagerRabbitService"
RUN dotnet build "HrManagerRabbitService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HrManagerRabbitService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HrManagerRabbitService.dll"]