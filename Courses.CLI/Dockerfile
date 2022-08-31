FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Courses.CLI/Courses.CLI.csproj", "Courses.CLI/"]
RUN dotnet restore "Courses.CLI/Courses.CLI.csproj"
COPY . .
WORKDIR "/src/Courses.CLI"
RUN dotnet build "Courses.CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Courses.CLI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Courses.CLI.dll"]
