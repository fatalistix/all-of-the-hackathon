FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HrManagerHttpService/HrManagerHttpService.csproj", "HrManagerHttpService/"]
RUN dotnet restore "HrManagerHttpService/HrManagerHttpService.csproj"

COPY . .
WORKDIR "/src/HrManagerHttpService"
RUN dotnet build "HrManagerHttpService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HrManagerHttpService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HrManagerHttpService.dll"]