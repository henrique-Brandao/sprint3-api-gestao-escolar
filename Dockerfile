FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Sprint3/Sprint3.csproj Sprint3/
RUN dotnet restore Sprint3/Sprint3.csproj

COPY Sprint3/ Sprint3/
RUN dotnet publish Sprint3/Sprint3.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Sprint3.dll"]
