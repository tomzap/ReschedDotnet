# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore first (better layer caching)
COPY ApiTodoDemo.csproj .
RUN dotnet restore

# copy everything else and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# copy published output only
COPY --from=build /app/publish .

# listen on 5151
ENV HTTP_PORTS=5151
EXPOSE 5151

ENTRYPOINT ["dotnet", "ApiTodoDemo.dll"]