# ===== Estágio 1: build do backend (.NET) =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src

COPY Back/src/Api.Domain/Api.Domain.csproj Back/src/Api.Domain/
COPY Back/src/Api.Application/Api.Application.csproj Back/src/Api.Application/
COPY Back/src/Api.Persistence/Api.Persistence.csproj Back/src/Api.Persistence/
COPY Back/src/Api/Api.csproj Back/src/Api/
RUN dotnet restore Back/src/Api/Api.csproj

COPY Back/src/ Back/src/
RUN dotnet publish Back/src/Api/Api.csproj -c Release -o /app/out

# ===== Estágio 2: build do frontend (Angular) =====
FROM node:22-alpine AS frontend
WORKDIR /front
COPY Front/package.json Front/package-lock.json Front/angular.json Front/tsconfig.json Front/tsconfig.app.json ./
COPY Front/src ./src
RUN npm ci
RUN npx ng build --configuration production

# ===== Estágio 3: runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=backend /app/out ./
COPY --from=frontend /front/dist/Client ./wwwroot

RUN mkdir -p /data/Resources/pdfs

EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]