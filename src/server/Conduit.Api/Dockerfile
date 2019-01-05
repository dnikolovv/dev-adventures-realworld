FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY /src .
RUN ls
WORKDIR server/Conduit.Api
RUN dotnet restore Conduit.Api.csproj
RUN dotnet publish Conduit.Api.csproj -c Release -o /app

FROM endeveit/docker-jq AS publish
WORKDIR /app
COPY --from=build /app .
RUN jq '.ConnectionStrings.DbConnectionString = "Server=db;Port=5432;Database=Conduit;User Id=postgres;Password=postgres;"' appsettings.json > tmp.$$.json && mv tmp.$$.json appsettings.json
RUN cat appsettings.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Conduit.Api.dll"]
