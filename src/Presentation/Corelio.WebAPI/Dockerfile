FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching on restore
COPY ["src/Aspire/Corelio.ServiceDefaults/Corelio.ServiceDefaults.csproj", "src/Aspire/Corelio.ServiceDefaults/"]
COPY ["src/Core/Corelio.SharedKernel/Corelio.SharedKernel.csproj", "src/Core/Corelio.SharedKernel/"]
COPY ["src/Core/Corelio.Domain/Corelio.Domain.csproj", "src/Core/Corelio.Domain/"]
COPY ["src/Core/Corelio.Application/Corelio.Application.csproj", "src/Core/Corelio.Application/"]
COPY ["src/Infrastructure/Corelio.Infrastructure/Corelio.Infrastructure.csproj", "src/Infrastructure/Corelio.Infrastructure/"]
COPY ["src/Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj", "src/Presentation/Corelio.WebAPI/"]

RUN dotnet restore "src/Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj"

# Copy remaining source and publish
COPY . .
RUN dotnet publish "src/Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Corelio.WebAPI.dll"]
