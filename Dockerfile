# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy your C# source code
COPY Program.cs .

# Initialize a console project and replace Program.cs
RUN dotnet new console --output . --name TGFApp --force
RUN dotnet publish -c Release -o out

# Use the smaller runtime image for the final container
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/out .

# Run the app
ENTRYPOINT ["dotnet", "TGFApp.dll"]
