# Step 1: runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

# Step 2: build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LucaHome.csproj", "./"] 
RUN dotnet restore "./LucaHome.csproj"
COPY . .
RUN dotnet build "LucaHome.csproj" -c Release -o /app/build

# Step 3: publish
FROM build AS publish
RUN dotnet publish "LucaHome.csproj" -c Release -o /app/publish

# Step 4: runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LucaHome.dll"] 
