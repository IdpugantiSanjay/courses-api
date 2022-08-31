FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Courses.API.csproj", "Courses.API/"]
RUN dotnet restore "Courses.API/Courses.API.csproj"
COPY . .
WORKDIR "/src/Courses.API"
RUN dotnet build "Courses.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Courses.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Courses.API.dll"]
