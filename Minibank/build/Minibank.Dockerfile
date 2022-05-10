FROM mcr.microsoft.com/dotnet/sdk:6.0 AS src

WORKDIR /src

COPY . .


WORKDIR src/

RUN dotnet build Minibank.Web -c Release -r linux-x64

RUN dotnet test Minibank.Core.Tests --no-build

RUN dotnet publish Minibank.Web -c Release -r linux-x64  --no-build -o /dist


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app

COPY --from=src /dist .

ENV ASPNETCORE_URLS=http://*:5001;http://*:5000

ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 5000 5001

ENTRYPOINT ["dotnet","Minibank.Web.dll"]