# Use the official .NET 8.0 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["agenticai.web/agenticai.web.csproj", "agenticai.web/"]
COPY ["agenticai.app/agenticai.app.csproj", "agenticai.app/"]
RUN dotnet restore "agenticai.web/agenticai.web.csproj"

COPY . .
WORKDIR "/src/agenticai.web"
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --no-self-contained

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "agenticai.web.dll"]