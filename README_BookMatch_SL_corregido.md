



#

## Matriz de evidencias de requerimientos mínimos

En esta tabla se indica dónde está implementado cada requisito. Algunas capturas todavía están pendientes de recopilar y se van a incorporar antes de la entrega final.

| Requerimiento mínimo | Estado | Enlace al código / controladores | Evidencia |
|---|---|---|---|
| **1. Modelo de datos y persistencia** | ✅ Implementado | [BookDbContext.cs](./Data/BookDbContext.cs#L26-L38) · [Relaciones de las entidades](./Data/BookDbContext.cs#L167-L206) · [Models](./Models/) | [Diagrama entidad-relación](./docs/evidencias/01-modelo-datos/BookMatch.png) |
| **2. Login, roles y avatar** | ✅ Implementado | [Login y logout](./Controllers/AuthController.cs#L118-L233) · [Perfil](./Controllers/AuthController.cs#L235-L300) · [Restricción por rol](./Controllers/UsuariosController.cs#L11-L52) · [ServicioAvatar.cs](./Services/ServicioAvatar.cs#L23-L76) | [Capturas de autenticación y perfiles](./docs/evidencias/02-autenticacion/) |
| **3. Manejo de archivos adicional al avatar** | ✅ Implementado | [Carga de portadas en LibroController](./Controllers/LibroController.cs#L120-L180) · [ServicioPortada.cs](./Services/ServicioPortada.cs#L18-L61) | Evidencia pendiente de incorporar. |
| **4. ABM con Vue.js + AJAX** | ✅ Implementado | [Acciones AJAX de CategoriaController](./Controllers/CategoriaController.cs#L22-L132) · [Vista y lógica Vue](./Views/Categoria/Index.cshtml#L102-L290) | [GIF del ABM](./docs/evidencias/04-vue/abm-categorias-vue-output.gif) · [Video](./docs/abm-categorias-vue.mp4) |
| **5. Paginación del lado del servidor** | ✅ Implementado | [Paginación del catálogo](./Controllers/LibroController.cs#L43-L65) · [Consulta paginada](./Repositories/RepositorioLibro.cs#L158-L181) | Evidencia pendiente de incorporar. |
| **6. Selección asíncrona de entidades relacionadas** | ✅ Implementado | [Búsqueda AJAX en IntercambioController](./Controllers/IntercambioController.cs#L102-L119) · [Vista de solicitud](./Views/Intercambio/Solicitar.cshtml#L26-L65) · [Lógica Fetch/Vue](./Views/Intercambio/Solicitar.cshtml#L80-L139) | Evidencia pendiente de incorporar. |
| **7. Web API REST + JWT** | ✅ Implementado | [Login y emisión del JWT](./Controllers/Api/AuthApiController.cs#L15-L118) · [Endpoints protegidos](./Controllers/Api/PerfilApiController.cs#L8-L32) | [Captura de Postman](./docs/evidencias/07-api-jwt/api-jwt-bearerpostmanseba.png) · [Colección Postman](./postman/BookMatch-SL.postman_collection.json) |

