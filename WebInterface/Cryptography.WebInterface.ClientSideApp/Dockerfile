FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebInterface/Cryptography.WebInterface.ClientSideApp/Cryptography.WebInterface.ClientSideApp.csproj", "Cryptography.WebInterface.ClientSideApp/"]
RUN dotnet restore "WebInterface/Cryptography.WebInterface.ClientSideApp/Cryptography.WebInterface.ClientSideApp.csproj"
COPY . .
WORKDIR "/src/Cryptography.WebInterface.ClientSideApp"
RUN dotnet build "Cryptography.WebInterface.ClientSideApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cryptography.WebInterface.ClientSideApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cryptography.WebInterface.ClientSideApp.dll"]
