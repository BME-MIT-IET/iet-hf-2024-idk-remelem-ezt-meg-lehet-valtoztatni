#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
       libc6-dev \
       libgdiplus \
       libx11-dev \
    && rm -rf /var/lib/apt/lists/*

# Install Microsoft core fonts
RUN echo "deb http://deb.debian.org/debian stable main contrib non-free" > /etc/apt/sources.list \
    && echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections \
    && apt-get update \
    && apt-get install -y \
        ttf-mscorefonts-installer \
    && apt-get clean \
    && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebShop.Api/WebShop.Api.csproj", "WebShop.Api/"]
COPY ["WebShop.Bll/WebShop.Bll.csproj", "WebShop.Bll/"]
COPY ["WebShop.Dal/WebShop.Dal.csproj", "WebShop.Dal/"]
RUN dotnet restore "WebShop.Api/WebShop.Api.csproj"
COPY . .
WORKDIR "/src/WebShop.Api"
RUN dotnet build "WebShop.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebShop.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShop.Api.dll"]