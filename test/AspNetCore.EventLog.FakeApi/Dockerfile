FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["test/AspNetCore.EventLog.FakeApi/AspNetCore.EventLog.FakeApi.csproj", "test/AspNetCore.EventLog.FakeApi/"]
RUN dotnet restore "test/AspNetCore.EventLog.FakeApi/AspNetCore.EventLog.FakeApi.csproj"
COPY . .
WORKDIR "/src/test/AspNetCore.EventLog.FakeApi"
RUN dotnet build "AspNetCore.EventLog.FakeApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCore.EventLog.FakeApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCore.EventLog.FakeApi.dll"]