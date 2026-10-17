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

### Paso 3: Ejecutar el proyecto

Desde la misma terminal, ejecutar en modo desarrollo:

```powershell
dotnet run --environment Development
```

Abrir en el navegador la dirección que aparece en la terminal.
