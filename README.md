## Configuración de la base de datos local

- Cada integrante debe configurar la base en su pc.
- No guardar la contraseña en `appsettings.json` ni en otros archivos del proyecto. La conexión se guarda con User Secrets, como se explica a continuación.

### Paso 1: Crear la base

1. Abrir la herramienta que uses para administrar MySQL.
2. Abrir o importar el archivo `Data/bookmatch_sl.sql`.
3. Ejecutar el script completo para crear `bookmatch_sl` y sus tablas.

Esto se hace una sola vez! Los datos que agregues quedan en tu computadora.

### Paso 2: Guardar la conexión

La conexión se guarda con User Secrets para no subir la contraseña a GitHub!

1. Abrir la terminal en la carpeta `BookMatch-SL`, donde está el archivo `.csproj`.
2. Escribir `powershell` y presionar Enter. OJO!! La línea del comando debe comenzar con `PS`.
3. Si el `.csproj` no tiene un `UserSecretsId`, ejecutar:

```powershell
dotnet user-secrets init
```

** Si ya lo tiene configurado, saltear ese comando.**

Para guardar tu conexión, reemplazar `TU_USUARIO` y `TU_CONTRASEÑA` por tus datos de MySQL conservando las comillas dobles de la linea:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=bookmatch_sl;User=TU_USUARIO;Password=TU_CONTRASEÑA;"
```

Si usás otro servidor o puerto, cambiar también `localhost` o `3306`.

Debe aparecer el mensaje:

```text
Successfully saved ConnectionStrings:DefaultConnection to the secret store.
```

### Paso 3: Generar y guardar la clave JWT

La app va a usar una clave secreta para firmar y validar los tokens JWT de la API. Cada uno debe generar su propia clave local.

```powershell
$key = [Convert]::ToBase64String(([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))); dotnet user-secrets set "Jwt:Key" $key

```

Si por cualquier motivo no te funcionó el paso a paso sería copiar las siguientes líneas una por una:

```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
```
```powershell
$bytes = New-Object byte[] 64
```
```powershell
$rng.GetBytes($bytes)
```
```powershell
$key = [Convert]::ToBase64String($bytes)
```
```powershell
$rng.Dispose()
```
Finalmente, guardar la clave en User Secrets:
```powershell
dotnet user-secrets set "Jwt:Key" $key
```

### Paso 4: Ejecutar el proyecto

El proyecto incluye migraciones de EF dentro de la carpeta Migrations. Estas migraciones contienen la estructura de la base de datos definida en BookDbContext.

Si la herramienta dotnet-ef no está instalada, hay que instalar la versión compatible con la versión de Entity Framework Core del proyecto:
```powershell
dotnet tool install --global dotnet-ef --version 9.0.0
```
Si ya está instalada, no es necesario repetir la instalación.
```powershell
dotnet ef database update --context BookDbContext
```
Una vez configurada la conexión y creada la base de datos vacía, ejecutar desde la carpeta del .csproj:
```powershell
dotnet ef database update --context BookDbContext
```

### Paso 5: Ejecutar el proyecto


Primero, restaurar los paquetes y comprobar que el proyecto compile:
```powershell
dotnet restore
```
```powershell
dotnet build
```
Desde la misma terminal, ejecutar en modo desarrollo:

```powershell
dotnet run --environment Development
```

Abrir en el navegador la dirección que aparece en la terminal.

### Probar la API

La colección de Postman con las pruebas de la API está en `postman/BookMatch-SL.postman_collection.json`.

1. Con el proyecto corriendo, importar ese archivo en Postman.
2. En las variables de la colección completar `email` y `password` con un usuario de prueba.
3. Ejecutar primero el Login. El token se guarda solo y lo usan los otros pedidos.

Si la app corre en otra dirección, cambiar también la variable `baseUrl`.
