# Use the ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# ✅ Correctly copy only the .csproj file
COPY APIGateway.csproj . 
RUN dotnet restore "APIGateway.csproj"

# ✅ Copy all source code
COPY . .

# ✅ Ensure the working directory is correct
WORKDIR /src
RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

# ✅ Publish the application
FROM build AS publish
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

# ✅ Final image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
