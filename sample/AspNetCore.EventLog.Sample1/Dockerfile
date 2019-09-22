FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["sample/AspNetCore.EventLog.Sample1/AspNetCore.EventLog.Sample1.csproj", "sample/AspNetCore.EventLog.Sample1/"]
RUN dotnet restore "sample/AspNetCore.EventLog.Sample1/AspNetCore.EventLog.Sample1.csproj"
COPY . .
WORKDIR "/src/sample/AspNetCore.EventLog.Sample1"
RUN dotnet build "AspNetCore.EventLog.Sample1.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCore.EventLog.Sample1.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCore.EventLog.Sample1.dll"]