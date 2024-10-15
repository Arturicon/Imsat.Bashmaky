#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Imsat.Bashmaky.Web/Imsat.Bashmaky.Web.csproj", "Imsat.Bashmaky.Web/"]
COPY ["Imsat.Bashmaky.Model/Imsat.Bashmaky.Model.csproj", "Imsat.Bashmaky.Model/"]
RUN dotnet restore "Imsat.Bashmaky.Web/Imsat.Bashmaky.Web.csproj"
COPY . .
WORKDIR "/src/Imsat.Bashmaky.Web"
RUN dotnet build "Imsat.Bashmaky.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Imsat.Bashmaky.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imsat.Bashmaky.Web.dll"]