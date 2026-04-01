# Rick and Morty — API interna (.NET 8)

API REST (.NET 8) que consume la API pública [Rick and Morty](https://rickandmortyapi.com/documentation), importa personajes a una base de datos local y gestiona solicitudes con un flujo de estados validado.

## Arquitectura

Se adoptó **Clean Architecture** con cuatro proyectos:

| Proyecto | Rol |
|----------|-----|
| **RickAndMorty.Domain** | Entidades, enumeraciones y reglas puras (transiciones de estado). |
| **RickAndMorty.Application** | Casos de uso (servicios de aplicación), DTOs, contratos (`IPersonajeRepository`, `IRickAndMortyApiClient`), validaciones FluentValidation y excepciones de negocio. |
| **RickAndMorty.Infrastructure** | EF Core (`ApplicationDbContext`), repositorios, cliente HTTP tipado hacia la API externa, migraciones. |
| **RickAndMorty.Api** | Controllers delgados, Swagger, health checks, middleware global de errores, composición de DI. |

Principios: **SOLID**, **inyección de dependencias**, **patrón Repository** (interfaces en Application, implementaciones en Infrastructure), **separación de responsabilidades**. La lógica de negocio y orquestación vive en Application; los controllers solo delegan.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (el archivo `global.json` fija la serie 8.0.x)
- (Opcional) Docker Desktop
- (Opcional) [EF Core tools](https://learn.microsoft.com/es-es/ef/core/cli/dotnet): `dotnet tool install dotnet-ef --global`

## Cómo ejecutar en local

```bash
cd src/RickAndMorty.Api
dotnet run
```

Por defecto se usa **SQLite** (`rickandmorty.dev.db` en Development). Los perfiles `http` / `https` en `launchSettings.json` fijan `DATABASE__PROVIDER=Sqlite` y la cadena al archivo local, así el modo local no hereda variables del `.env` de Docker (por ejemplo `Server=sql`). Swagger: por ejemplo `http://localhost:5294/swagger`.

Variables útiles (sobreescriben `appsettings` si las exporta en la shell **sin** usar perfil de lanzamiento):

- `ConnectionStrings__DefaultConnection`
- `Database__Provider`: `Sqlite` o `SqlServer`
- `RickAndMortyApi__BaseUrl` (default `https://rickandmortyapi.com/api/`)

### Simular credenciales “como Key Vault” (local / pruebas)

En **Azure App Service**, una referencia tipo `@Microsoft.KeyVault(SecretUri=...)` se resuelve y la aplicación recibe **variables de entorno** con el valor final; la app no ve la sintaxis de Key Vault.

Para recrear ese escenario sin commitear secretos:

1. **`docker compose`**  
   Copie `.env.example` a `.env` en la raíz del repo y defina al menos `CONNECTIONSTRINGS__DEFAULTCONNECTION` (y el resto si aplica). Compose sustituye `${VAR:-valorPorDefecto}` en `docker-compose.yml`; si omite `.env`, siguen valiendo los valores por defecto del compose.

2. **`dotnet run`**  
   Exporte las mismas claves (`ConnectionStrings__DefaultConnection`, etc.) en el shell o use [User Secrets](https://learn.microsoft.com/es-es/aspnet/core/security/app-secrets) (`dotnet user-secrets set ...`).

3. **Key Vault real**  
   Defina `KeyVault__VaultUri` o `KEYVAULT_VAULTURI` con la URL del vault (`https://<nombre>.vault.azure.net/`). La API carga secretos con `DefaultAzureCredential` (por ejemplo `az login` en desarrollo, **Managed Identity** en Azure). En el vault, un secreto llamado `ConnectionStrings--DefaultConnection` se mapea a `ConnectionStrings:DefaultConnection`.

### Azure SQL (simulación / producción)

En `appsettings.Production.json` el proveedor sugerido es `SqlServer` y la cadena vacía: debe aportarse solo por **variables de entorno** o **Azure App Configuration / Key Vault**, por ejemplo:

`ConnectionStrings__DefaultConnection=Server=tcp:xxx.database.windows.net,1433;Initial Catalog=RickAndMorty;...`

## Docker

Desde la raíz del repositorio:

```bash
docker compose up --build
```

En segundo plano:

```bash
docker compose up --build -d
```

- **API**: `http://localhost:8080` (Swagger en `/swagger`, health en `/health`).
- **Azure SQL Edge** (motor compatible con T-SQL) en el puerto **14333** (usuario `sa`, contraseña la definida en `docker-compose.yml`; **cámbiela en entornos reales**). Se usa Edge en lugar de `mssql/server` para que el stack funcione también en **Apple Silicon (ARM64)**.
- El servicio SQL tiene **healthcheck** y la API espera a que esté **healthy** antes de arrancar. Las migraciones en la API **reintentan** si la base aún no responde.

Si cambió de imagen SQL o hay datos incompatibles en el volumen: `docker compose down -v` y vuelva a levantar el stack.

Las migraciones están pensadas para **SQLite y SQL Server / Azure SQL Edge** (DDL distinto por proveedor en `InitialCreate`). Si **POST `/api/personajes/importar`** devuelve **500** en Docker (p. ej. conflicto de tipos `datetime2` / `text`), la base suele ser antigua: `docker compose down -v`, luego `docker compose up --build`. En **Development**, la API puede devolver el detalle del error de persistencia en `errors[]` del JSON de respuesta.

Las credenciales de desarrollo para Compose están en **`.env`** (no se versiona; parta de **`.env.example`** con `cp .env.example .env`). `docker compose` las carga al sustituir variables en `docker-compose.yml`.

## Entity Framework Core

Migraciones en el proyecto **Infrastructure** (`Persistence/Migrations`).

```bash
export PATH="$PATH:$HOME/.dotnet/tools"   # si dotnet-ef no está en PATH

dotnet ef migrations add NombreMigracion \
  --project src/RickAndMorty.Infrastructure/RickAndMorty.Infrastructure.csproj \
  --startup-project src/RickAndMorty.Api/RickAndMorty.Api.csproj

dotnet ef database update \
  --project src/RickAndMorty.Infrastructure/RickAndMorty.Infrastructure.csproj \
  --startup-project src/RickAndMorty.Api/RickAndMorty.Api.csproj
```

En desarrollo, `Program.cs` aplica migraciones automáticamente al iniciar (`MigrateAsync`).

## Endpoints principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/personajes/importar` | Importa por `externalIds` o, si van vacíos, páginas de la API (`maxPaginasApi`, máx. 50). |
| GET | `/api/personajes` | Lista con filtros (`nombre`, `estado`, `especie`, `genero`, `origen`) y paginación (`page`, `pageSize`). |
| GET | `/api/personajes/{id}` | Detalle por id interno. |
| POST | `/api/solicitudes` | Crea solicitud en estado `Pendiente`. |
| GET | `/api/solicitudes` | Lista solicitudes. |
| GET | `/api/solicitudes/{id}` | Detalle. |
| PATCH | `/api/solicitudes/{id}/estado` | Cambio de estado con reglas: Pendiente→EnProceso/Rechazada; EnProceso→Aprobada/Rechazada. |
| GET | `/api/reportes/solicitudes-resumen` | Totales por estado. |
| GET | `/health` | Health check (+ comprobación de BD). |

## Reglas de estado de solicitudes

Transiciones permitidas:

- `Pendiente` → `EnProceso`
- `Pendiente` → `Rechazada`
- `EnProceso` → `Aprobada`
- `EnProceso` → `Rechazada`

Cualquier otra transición devuelve **409 Conflict** con mensaje explícito (vía `BusinessRuleException` y middleware).

## Pruebas

```bash
dotnet test RickAndMorty.sln
```

Incluye al menos tres pruebas unitarias (xUnit + Moq + FluentAssertions): importación de personajes, creación de solicitud y validación de cambio de estado.

## CI/CD básico

Workflow GitHub Actions: `.github/workflows/ci.yml` (restore, build, test en .NET 8).

## Estructura del repositorio

```
RickAndMorty.sln
src/
  RickAndMorty.Api/
  RickAndMorty.Application/
  RickAndMorty.Domain/
  RickAndMorty.Infrastructure/
tests/
  RickAndMorty.Tests/
Dockerfile
docker-compose.yml
```

Solución principal: **RickAndMorty.sln**.