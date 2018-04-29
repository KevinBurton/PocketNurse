FROM microsoft/aspnetcore AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY PocketNurse.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT Docker
WORKDIR /app
COPY --from=publish /app .
COPY appsettings.Docker.json appsettings.json
ENTRYPOINT ["dotnet", "PocketNurse.dll"]
