# Dockerfile

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

# Run the app on container startup
# Use your project name for the second parameter
# e.g. MyProject.dll
# ENTRYPOINT [ "dotnet", "larp_server.dll" ]

# ODKOMENTOWAC ZEBY DZIALALO LOKALNIE!!!
# ENTRYPOINT [ "dotnet", "larp_server.dll" ]
# Use the following instead for Heroku

# ODKOMENTOWAC ZEBY DZIALALO NA HEROKU!!!
CMD ASPNETCORE_URLS=http://*:$PORT dotnet larp_server.dll
