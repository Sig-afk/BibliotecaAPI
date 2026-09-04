# Um único Dockerfile na raiz produz as duas imagens da aplicação.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src
COPY BibliotecaAPI/BibliotecaAPI.csproj BibliotecaAPI/
RUN dotnet restore BibliotecaAPI/BibliotecaAPI.csproj
COPY BibliotecaAPI/ BibliotecaAPI/
WORKDIR /src/BibliotecaAPI
RUN dotnet publish BibliotecaAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS backend
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=backend-build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BibliotecaAPI.dll"]

FROM nginx:1.27-alpine AS frontend
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY frontend/index.html frontend/styles.css frontend/app.js /usr/share/nginx/html/
EXPOSE 80
