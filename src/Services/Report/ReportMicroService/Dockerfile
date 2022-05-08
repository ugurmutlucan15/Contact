#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Services/Report/ReportMicroService/ReportMicroService.csproj", "src/Services/Report/ReportMicroService/"]
COPY ["src/Common/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/Common/EventBusRabbitMQ/"]
RUN dotnet restore "src/Services/Report/ReportMicroService/ReportMicroService.csproj"
COPY . .
WORKDIR "/src/src/Services/Report/ReportMicroService"
RUN dotnet build "ReportMicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportMicroService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportMicroService.dll"]