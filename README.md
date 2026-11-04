# Bookmatch-SL 📚

Plataforma web para el intercambio de libros físicos entre usuarios de San Luis.

Bookmatch-SL permite explorar un catálogo público de libros, publicar ejemplares propios, proponer intercambios y comunicarse con otros usuarios mediante un chat asociado a cada intercambio.

El sistema cuenta con autenticación, roles de usuario y administrador, gestión de libros, auditoría de operaciones y una API REST protegida mediante JWT.

---
### ⚠️ Nota Aclaratoria (Disclaimer)
Los usuarios de prueba y los libros cargados en el sistema tienen un tinte humorístico debido a una particularidad del equipo de desarrollo: **todos los integrantes que colaboramos en este proyecto compartimos el apellido "Castro"**. 

Las referencias o nombres utilizados (como Fidel Castro, Nelson Castro, etc.) se incluyeron exclusivamente como un juego de palabras interno y de prueba para el entorno de desarrollo. Bajo ningún punto pretenden ofender, hacer alusión política, ni fijar posición ideológica de ningún tipo.

---
### Tecnologías utilizadas

- **ASP.NET Core MVC (.NET 10):** estructura principal de la aplicación.
- **C# y Entity Framework Core:** lógica del servidor y acceso a los datos.
- **MySQL / MariaDB:** almacenamiento de usuarios, libros, categorías, intercambios, mensajes y auditoría. El dump incluido se exportó desde MariaDB 10.4.32.
- **Razor, HTML, CSS y Bootstrap:** vistas e interfaz.
- **Vue 3 + JavaScript Fetch:** ABM de categorías y búsqueda asíncrona de libros para proponer intercambios.
- **SignalR:** chat en tiempo real asociado a los intercambios.
- **Cookies:** autenticación de la aplicación MVC.
- **JWT:** autenticación de los endpoints de la API REST.
- **Postman:** pruebas de los endpoints de la API.

---

## Cómo descargar y ejecutar el proyecto localmente

Estos pasos están pensados para ejecutar el proyecto desde una computadora nueva, por ejemplo, después de clonarlo desde GitHub.

### Requisitos previos

Antes de empezar, necesitás tener instalado:

- Git.
- **.NET SDK 10**, porque el proyecto tiene como destino `net10.0`.
- MySQL o MariaDB, con el servicio iniciado.
- Una herramienta para administrar la base de datos, por ejemplo, HeidiSQL, DBeaver, MySQL Workbench o phpMyAdmin.
- Postman, solamente si también querés probar la API.

### 1. Clonar el repositorio

Abrí una terminal y ejecutá:

```bash
git clone https://github.com/florenciacastro95/BookMatch-SL.git
cd BookMatch-SL
```

Los comandos siguientes se ejecutan desde la carpeta del proyecto, donde está `Bookmatch-SL.csproj`.

### 2. Restaurar las imágenes de prueba

Las portadas y los avatares se almacenan en `wwwroot/uploads/`. Esa carpeta está incluida en el `.gitignore`, por lo que las imágenes no se descargan al clonar el repositorio.

Para que los registros del SQL muestren sus imágenes, también se entrega por separado el archivo **`uploads_bookmatch.zip`**. Descomprimí su contenido dentro de `wwwroot/uploads/`. El ZIP contiene las carpetas `portadas` y `avatares`, por lo que la estructura final debe quedar así:

```text
wwwroot/
└── uploads/
    ├── portadas/
    │   ├── imagen-1.jpg
    │   └── ...
    └── avatares/
        ├── avatar-1.jpg
        └── ...
```

En Windows, si el ZIP está en la carpeta Descargas, podés hacerlo desde PowerShell, parado en la carpeta del proyecto:

```powershell
New-Item -ItemType Directory -Force ".\wwwroot\uploads" | Out-Null
Expand-Archive -Path "$HOME\Downloads\uploads_bookmatch.zip" -DestinationPath ".\wwwroot\uploads" -Force
```

También podés descomprimirlo con 7-Zip o el Explorador de archivos, siempre que las carpetas `portadas` y `avatares` queden directamente dentro de `wwwroot/uploads/`.

**Importante:** no cambies los nombres de los archivos. La base de datos guarda las rutas de las imágenes (por ejemplo, `/uploads/portadas/nombre-del-archivo.jpg`) y la aplicación necesita encontrar esos mismos archivos.

### 3. Crear e importar la base de datos

El archivo `Data/bookmatch_sl.sql` contiene la estructura de las tablas y los datos de prueba.

Primero, creá una base de datos vacía llamada `bookmatch_sl`, con codificación `utf8mb4` y collation `utf8mb4_general_ci`. Por ejemplo, podés ejecutar:

```sql
CREATE DATABASE bookmatch_sl
CHARACTER SET utf8mb4
COLLATE utf8mb4_general_ci;
```

Después, desde tu administrador de MySQL/MariaDB:

1. Seleccioná la base de datos `bookmatch_sl`.
2. Entrá en la opción **Importar**.
3. Elegí el archivo `Data/bookmatch_sl.sql` del proyecto.
4. Ejecutá la importación completa.

El script crea las seis tablas principales (`usuario`, `categoria`, `libro`, `intercambio`, `mensaje` y `auditoria`) y carga los registros incluidos para probar la aplicación.

**Importante sobre las migraciones:** para esta entrega, importá el SQL y no ejecutes después `dotnet ef database update`. El dump ya crea las tablas y carga los datos, pero no incluye la tabla `__EFMigrationsHistory`. Si se ejecuta la migración inicial sobre esa base restaurada, Entity Framework puede intentar crear tablas que ya existen. Las migraciones están en el proyecto, pero no hacen falta para restaurar esta base de prueba.

Si ya tenés una base `bookmatch_sl` con tablas, no importes el archivo encima sin antes hacer una copia de seguridad. Lo más simple para una instalación limpia es crear una base vacía y después importar el dump.


---

### 4. Configurar la conexión con User Secrets

La cadena de conexión no se guarda en `appsettings.json`, para evitar subir usuarios y contraseñas de MySQL al repositorio.

El proyecto ya tiene configurado un `UserSecretsId` en `Bookmatch-SL.csproj`, así que **no hace falta ejecutar `dotnet user-secrets init`**.

Desde PowerShell, en la carpeta donde está el `.csproj`, ejecutá el siguiente comando. Reemplazá `TU_USUARIO` y `TU_CONTRASEÑA` por los datos de tu instalación local:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=bookmatch_sl;User=TU_USUARIO;Password=TU_CONTRASEÑA;"
```

Si tu servidor utiliza otro puerto o dirección, modificá `3306` o `localhost` según corresponda.

### 5. Generar la clave JWT local

La API necesita una clave privada para firmar y validar los tokens. Cada instalación debe generar su propia clave; no hay que compartirla ni subirla a GitHub.

En PowerShell, ejecutá estas líneas una por una:

```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 32
$rng.GetBytes($bytes)
$rng.Dispose()
$key = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:Key" $key
```

`Jwt:Issuer` y `Jwt:Audience` ya están definidos en `appsettings.json`, así que para la configuración local solo hace falta guardar `Jwt:Key`.

### 6. Restaurar paquetes, compilar y ejecutar

Desde la terminal, en la carpeta del `.csproj`, ejecutá:

```powershell
dotnet restore
dotnet build
```

Para usar el perfil HTTPS del proyecto, primero podés confiar el certificado de desarrollo de .NET (Windows puede pedir confirmación):

```powershell
dotnet dev-certs https --trust
```

Después, ejecutá:

```powershell
dotnet run --launch-profile https
```

La aplicación queda disponible en:

```text
https://localhost:7259
```

El perfil también configura el entorno `Development`. Si el navegador no se abre automáticamente, copiá la dirección anterior.

---

## 7. Usuarios de prueba

El SQL incluye usuarios de prueba con los roles **Administrador** y **Usuario**. Para probar el inicio de sesión, usá las credenciales de prueba que se indican a continuación.

> **Nota:** Todas las cuentas listadas a continuación son ficticias y se utilizan exclusivamente para pruebas de desarrollo, roles y permisos dentro de la aplicación.

---

## ⚡ Administradores (ADMINS)

| Nombre | Correo Electrónico | Contraseña | Rol |
| :--- | :--- | :--- | :--- |
| **Eri Castro** | `eriCastro@ulp.edu.ar` | `ulp12345` | `Admin` |
| **Sebas Castro** | `sebasCastro@ulp.edu.ar` | `ulp12345` | `Admin` |

---

## 👤 Usuarios Comunes (USERS)

| Nombre | Correo Electrónico | Contraseña | Rol |
| :--- | :--- | :--- | :--- |
| **Flor Castro** | `florCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Grupo (bi)Castro** | `grupoBiCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Inmobiliaria Castro** | `inmobiliariaCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Cristian Castro** | `tuAmorEsAzulCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Nelson Castro** | `deNingunaManeraCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Fidel Castro** | `queNoLoAbsuelvaLaHistoriaCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Roli Castro** | `roliCastro@ulp.edu.ar` | `ulp12345` | `User` |
| **Luciano Castro** | `holaguapaCastro@ulp.edu.ar` | `ulp12345` | `User` |

Las contraseñas de la base están almacenadas como hashes, por lo que no se pueden recuperar leyendo el SQL. Antes de entregar, hay que confirmar las contraseñas originales de prueba o asignarles nuevas y actualizar esta tabla con las claves en texto plano destinadas exclusivamente a la evaluación.

### 8. Probar la API con Postman

La colección está en `postman/BookMatch-SL.postman_collection.json`.

1. Abrí Postman e importá ese archivo.
2. En las variables de la colección, configurá `baseUrl` como `https://localhost:7259`.
3. Completá `email` y `password` con las credenciales de un usuario de prueba.
4. Ejecutá primero **Login**. La colección guarda el token recibido.
5. Después ejecutá **Mi perfil** para probar un endpoint protegido y **Solo administrador** con una cuenta Administrador.

Si cambiaste el puerto o la dirección de la aplicación, actualizá también `baseUrl`.

**Nota:** Vue 3 se carga desde `unpkg.com`; para probar el ABM de categorías y la búsqueda reactiva, el navegador necesita conexión a Internet.

---

# Matriz de evidencias de requerimientos mínimos

| Requerimiento mínimo | Estado | Enlace al código / controladores | Evidencia |
|---|---|---|---|
| **1. Modelo de datos y persistencia** | ✅ Cumplido | [BookDbContext.cs](./Data/BookDbContext.cs) · [Models](./Models/) | [Ver DER para la DB de BookMatch](./docs/evidencias/01-modelo-datos/BookMatch.png) |
| **2. Login, roles y avatar** | ✅ Cumplido | [AuthController.cs](./Controllers/AuthController.cs) · [UsuariosController.cs](./Controllers/UsuariosController.cs) | [Ver vista no autenticada](./docs/evidencias/02-autenticacion/noautenticado_Evidencia_punt2.png) • [Ver perfil rol usuario](./docs/evidencias/02-autenticacion/perfilLucianoCastro_Evidencia_punt2.png) • [Ver vista rol admin](./docs/evidencias/02-autenticacion/perfilEriCastro_Evidencia_punt2.png) |
| **3. Manejo de archivos adicional al avatar** | ✅ Cumplido | [LibroController.cs](./Controllers/LibroController.cs) · [ServicioPortada.cs](./Services/ServicioPortada.cs) | [Ver demostración](./docs/evidencias/3-manejo-de-archivos/) |
| **4. ABM con Vue.js + AJAX** | ✅ Cumplido | [Código Vue.js implementado para categoria](./Views/Categoria/Index.cshtml) | [Ver demostración](./docs/evidencias/04-vue/abm-categorias-vue-output.gif) | |
| **5. Paginación del lado del servidor** | ✅ Cumplido | [RepositorioLibro.cs](./Repositories/RepositorioLibro.cs) | [Ver evidencias](./docs/evidencias/05-paginacion/) |
| **6. Selección asíncrona de entidades relacionadas** | ✅ Cumplido | [IntercambioController.cs](./Controllers/IntercambioController.cs) | [Ver evidencias](./docs/evidencias/06-busqueda-ajax/) |
| **7. Web API REST + JWT** | ✅ Implementado | [AuthApiController.cs](./Controllers/Api/AuthApiController.cs) · [PerfilApiController.cs](./Controllers/Api/PerfilApiController.cs) | [Ver evidencias](./docs/evidencias/07-api-jwt/api-jwt-bearerpostmanseba.png) |

---

## Un poco más sobre la implementación

### 1. Modelo de datos y persistencia

El proyecto utiliza Entity Framework Core para trabajar con las entidades relacionadas. Las entidades principales son:

- Usuario
- Categoría
- Libro
- Intercambio
- Mensaje
- Auditoría

Hay relaciones 1:N, por ejemplo, entre un usuario y sus libros, y entre un intercambio y sus mensajes. La entidad `Intercambio` también relaciona dos usuarios y dos libros: el usuario que propone, el usuario que recibe la propuesta, el libro ofrecido y el libro solicitado.

**Código:** [BookDbContext.cs](./Data/BookDbContext.cs) · [Carpeta Models](./Models/)

**Diagrama:** [BookMatch.png](./docs/evidencias/01-modelo-datos/BookMatch.png)

### 2. Login, roles y avatar

La aplicación web utiliza autenticación mediante cookies y dos roles: `Usuario` y `Administrador`. Las acciones administrativas se restringen desde el backend, no solamente ocultando botones en las vistas.

Los usuarios pueden cargar y actualizar su avatar. El servicio valida el tipo y el tamaño del archivo y lo guarda en `wwwroot/uploads/avatares/`.

**Código:** [AuthController.cs](./Controllers/AuthController.cs) · [UsuariosController.cs](./Controllers/UsuariosController.cs) · [ServicioAvatar.cs](./Services/ServicioAvatar.cs)

### 3. Manejo de archivos

Además del avatar, se pueden cargar imágenes de portada para los libros publicados. El servicio valida la extensión, el tamaño y la firma del archivo, y guarda la imagen en `wwwroot/uploads/portadas/`. En la base de datos se conserva la ruta relativa mediante `PortadaUrl`.

**Código:** [LibroController.cs](./Controllers/LibroController.cs) · [ServicioPortada.cs](./Services/ServicioPortada.cs)

### 4. ABM con Vue.js + AJAX

El módulo de categorías permite listar, crear, modificar y desactivar categorías mediante Vue 3 y peticiones AJAX con Fetch. Las acciones del controlador responden con JSON y la vista actualiza los datos sin recargar la página. El listado también solicita las categorías por página.

**Código:** [CategoriaController.cs](./Controllers/CategoriaController.cs) · [Views/Categoria/Index.cshtml](./Views/Categoria/Index.cshtml)

**Demostraciones:** [GIF](./docs/evidencias/04-vue/abm-categorias-vue-output.gif) · [Video](https://drive.google.com/file/d/1smFCAFhAnA1YF_ahjUU9JZEaQ_UgSl4s/view?usp=sharing)

### 5. Paginación del lado del servidor

Los listados principales solicitan los registros por páginas. Para el catálogo, el controlador calcula la cantidad de páginas y el repositorio utiliza `Skip()` y `Take()` para consultar solamente los libros de la página solicitada.

**Código:** [LibroController.cs](./Controllers/LibroController.cs) · [RepositorioLibro.cs](./Repositories/RepositorioLibro.cs)

### 6. Selección asíncrona de entidades relacionadas

Al preparar una propuesta de intercambio, el usuario busca uno de sus libros por título o autor. La vista envía el texto al servidor mediante Fetch y el controlador devuelve únicamente los resultados encontrados en JSON; no se carga de antemano una lista completa para llenar un desplegable masivo.

**Código:** [IntercambioController.cs](./Controllers/IntercambioController.cs) · [Solicitar.cshtml](./Views/Intercambio/Solicitar.cshtml) · [RepositorioLibro.cs](./Repositories/RepositorioLibro.cs)

### 7. Web API REST + JWT

La API cuenta con un endpoint de login que valida las credenciales y genera un token JWT. Los endpoints de perfil requieren ese token como `Bearer`; además, hay un endpoint restringido al rol Administrador.

**Código:** [AuthApiController.cs](./Controllers/Api/AuthApiController.cs) · [PerfilApiController.cs](./Controllers/Api/PerfilApiController.cs)

**Pruebas:** [Colección de Postman](./postman/BookMatch-SL.postman_collection.json)

---

## Decisiones y desafíos durante el desarrollo

- **Archivos e imágenes:** las imágenes se guardan físicamente en el servidor y la base conserva sus rutas. Por eso, para restaurar la base de demostración también hay que recuperar las carpetas de portadas y avatares con sus nombres originales.
- **Auditoría:** se utiliza el `ChangeTracker` de Entity Framework Core para registrar altas, modificaciones y bajas. Los valores anteriores y nuevos se guardan como JSON porque las entidades tienen propiedades diferentes; así se conserva una estructura flexible para los distintos cambios.
- **Búsquedas y listados:** se trabajó para que el servidor devuelva solamente los registros necesarios para cada página o búsqueda, en lugar de cargar listas completas en el navegador.
- **Autenticación:** la aplicación MVC utiliza cookies, mientras que la API utiliza JWT. Son dos mecanismos distintos que conviven en el mismo proyecto.
- **Trabajo en equipo:** se integraron los distintos módulos manteniendo una estructura común de modelos, controladores, repositorios, servicios y vistas.

---

## Estructura principal del proyecto

```text
Bookmatch-SL/
├── Controllers/       # Controladores MVC y de la API
├── Data/              # BookDbContext y dump SQL
├── Migrations/        # Migraciones de Entity Framework Core
├── Models/            # Entidades, enums y ViewModels
├── Repositories/      # Acceso a datos
├── Services/          # Servicios de archivos
├── Views/             # Vistas Razor
├── Hubs/              # Chat con SignalR
├── wwwroot/           # Archivos estáticos y uploads locales
├── postman/           # Colección para probar la API
└── docs/              # Diagramas y evidencias
```

---

## Nota sobre la carpeta `uploads`

La carpeta `wwwroot/uploads/` está excluida del repositorio mediante `.gitignore` porque contiene archivos que se generan y cambian mientras se usa la aplicación. Para la evaluación se entrega un paquete separado con las imágenes de prueba. De esta forma, el repositorio conserva el código y los archivos necesarios para documentarlo, mientras que cada instalación puede tener sus propios archivos cargados.
