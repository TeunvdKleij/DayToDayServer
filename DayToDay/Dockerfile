FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
WORKDIR /src
RUN dotnet build --no-restore -c Release -o /app/build
FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Data Source=/app/app.db"

# Run the application
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
