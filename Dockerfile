FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
EXPOSE 80

RUN git clone https://github.com/michaltalaga/RightmoveDownloader.git
WORKDIR RightmoveDownloader
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 as final
COPY --from=build /app /app
COPY RightmoveDownloader/google-service-account.json /app
WORKDIR /app

ENTRYPOINT ["bash"]
#ENTRYPOINT ["dotnet", "RightmoveDownloader.dll"]


# FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
# WORKDIR /app
# EXPOSE 80

# FROM microsoft/dotnet:2.1-sdk AS build
# RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
# RUN apt-get install -y nodejs
# #WORKDIR /
# #COPY "/Portal/Portal.csproj" Portal2/
# #RUN dotnet restore "Portal2/Portal.csproj"
# #COPY Portal Portal2
# #WORKDIR /Portal2
# #RUN dotnet build "Portal.csproj" -c Release -o /app
# #
# FROM build AS publish
# WORKDIR /
# COPY Portal Portal
# WORKDIR /Portal
# RUN dotnet publish "Portal.csproj" -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "Portal.dll"]