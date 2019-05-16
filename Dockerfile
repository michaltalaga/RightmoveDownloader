FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
EXPOSE 80

COPY RightmoveDownloader /RightmoveDownloader
WORKDIR /RightmoveDownloader
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 as final
COPY --from=build /app /app
WORKDIR /app
RUN sed -i '/^CipherString/d' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "RightmoveDownloader.dll"]