FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EmployeeHttpService/EmployeeHttpService.csproj", "EmployeeHttpService/"]
RUN dotnet restore "EmployeeHttpService/EmployeeHttpService.csproj"

COPY . .
WORKDIR "/src/EmployeeHttpService"
RUN dotnet build "EmployeeHttpService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeHttpService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "EmployeeHttpService.dll"]