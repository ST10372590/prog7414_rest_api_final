FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

# Copy csproj and restore from UniVerse folder
COPY UniVerse/*.csproj ./UniVerse/
RUN dotnet restore UniVerse/UniVerse.csproj

# Copy everything else
COPY . ./

RUN dotnet publish UniVerse/UniVerse.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "UniVerse.dll"]
