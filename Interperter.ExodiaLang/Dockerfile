FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Grammer.ParserGenerator/Grammer.ParserGenerator.csproj", "Grammer.ParserGenerator/"]
RUN dotnet restore "Grammer.ParserGenerator/Grammer.ParserGenerator.csproj"
COPY . .
WORKDIR "/src/Grammer.ParserGenerator"
RUN dotnet build "Grammer.ParserGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Grammer.ParserGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Grammer.ParserGenerator.dll"]
