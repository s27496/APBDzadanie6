FROM mcr.microsoft.com/dotnet/sdk:8.0
LABEL authors="Szymon Romnako S27496"

WORKDIR /app/Zadanie6

# Dependencies
COPY ./Zadanie6/Zadanie6.csproj /app/Zadanie6

RUN dotnet restore

# Project
COPY ./Zadanie6 /app/Zadanie6

RUN dotnet publish -c Release -o /app/out

ENTRYPOINT ["dotnet", "/app/out/Zadanie6.dll", "--launch-profile", "http"]
