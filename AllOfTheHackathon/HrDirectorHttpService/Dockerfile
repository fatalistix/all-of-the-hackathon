FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HrDirectorHttpService/HrDirectorHttpService.csproj", "HrDirectorHttpService/"]
RUN dotnet restore "HrDirectorHttpService/HrDirectorHttpService.csproj"

COPY . .
WORKDIR "/src/HrDirectorHttpService"
RUN dotnet build "HrDirectorHttpService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HrDirectorHttpService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HrDirectorHttpService.dll"]