# Use the ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK to build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# ✅ Copy csproj directly (since you're already in TweetManagement/)
COPY TweetManagement.csproj .
RUN dotnet restore "TweetManagement.csproj"

# ✅ Copy rest of the source code
COPY . .

# ✅ Build
RUN dotnet build "TweetManagement.csproj" -c Release -o /app/build

# ✅ Publish
FROM build AS publish
RUN dotnet publish "TweetManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ✅ Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TweetManagement.dll"]
