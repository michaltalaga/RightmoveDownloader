FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
EXPOSE 80

COPY RightmoveDownloader /RightmoveDownloader
WORKDIR /RightmoveDownloader
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final
COPY --from=build /app /app
WORKDIR /app
RUN sed -i '/^CipherString/d' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "RightmoveDownloader.dll"]