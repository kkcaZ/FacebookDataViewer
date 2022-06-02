FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /FacebookDataViewer

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /FacebookDataViewer/out .
ENTRYPOINT ["dotnet", "FacebookDataViewer.dll"]
