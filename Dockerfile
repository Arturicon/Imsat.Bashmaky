FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

COPY *.sln .
COPY nuget.local/. ./nuget.local/
COPY nuget.local.restore.config .
# Folder/*.csproj ./Folder/
COPY Imsat.Bashmaky.Model/*.csproj ./Imsat.Bashmaky.Model/
COPY Imsat.Bashmaky.Web/*.csproj ./Imsat.Bashmaky.Web/


RUN ls --recursive /source/nuget.local/

#RUN dotnet restore Prj/Prj.csproj --configfile /source/nuget.local.restore.config
RUN dotnet restore Imsat.Bashmaky.Model/Imsat.Bashmaky.Model.csproj --configfile /source/nuget.local.restore.config
RUN dotnet restore Imsat.Bashmaky.Web/Imsat.Bashmaky.Web.csproj --configfile /source/nuget.local.restore.config


# copy everything else and build app
#COPY Prj/. ./Prj/
COPY Imsat.Bashmaky.Model/. ./Imsat.Bashmaky.Model/
COPY Imsat.Bashmaky.Web/. ./Imsat.Bashmaky.Web/


WORKDIR /source/Imsat.Bashmaky.Web
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Imsat.Bashmaky.Web.dll"]