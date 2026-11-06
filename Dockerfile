FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ConcesionarioMotos.Web/ConcesionarioMotos.Web.csproj ConcesionarioMotos.Web/
RUN dotnet restore ConcesionarioMotos.Web/ConcesionarioMotos.Web.csproj

COPY Modelos.cs DatosIniciales.cs ./
COPY ConcesionarioMotos.Web/ ConcesionarioMotos.Web/
RUN dotnet publish ConcesionarioMotos.Web/ConcesionarioMotos.Web.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
USER app
EXPOSE 8080
CMD ["sh", "-c", "exec dotnet ConcesionarioMotos.Web.dll --urls http://0.0.0.0:${PORT:-8080}"]
