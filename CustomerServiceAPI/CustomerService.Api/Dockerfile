#Stage1: BUild
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
#Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore
#Copy remaining files and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish
#Stage2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
#Copr published output from build stage
COPY --from=build /app/publish .
#ENTRYPOINT must be JSON array (exec form)
ENTRYPOINT ["dotnet", "CustomerService.Api.dll"]