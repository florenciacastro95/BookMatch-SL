-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 04-11-2026 a las 08:15:30
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.1.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `bookmatch_sl`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `auditoria`
--

CREATE TABLE `auditoria` (
  `id_auditoria` int(11) NOT NULL,
  `id_usuario` int(11) DEFAULT NULL,
  `accion` varchar(100) NOT NULL,
  `entidad` varchar(100) NOT NULL,
  `id_entidad` int(11) DEFAULT NULL,
  `fecha_hora` datetime(6) NOT NULL,
  `datos_anteriores` longtext DEFAULT NULL,
  `datos_nuevos` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `auditoria`
--

INSERT INTO `auditoria` (`id_auditoria`, `id_usuario`, `accion`, `entidad`, `id_entidad`, `fecha_hora`, `datos_anteriores`, `datos_nuevos`) VALUES
(1, NULL, 'ALTA', 'Usuario', 1, '2026-11-04 03:26:06.041800', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/06d3b7503f0e4a47a45b34905a59034c.jpg\",\"CreatedAt\":\"2026-11-04T03:26:06.0394197Z\",\"Email\":\"ericastro@ulp.edu.ar\",\"Nombre\":\"Eri Castro\",\"Rol\":\"Usuario\"}'),
(2, NULL, 'INICIO_SESION_RECHAZADO', 'Usuario', NULL, '2026-11-04 03:26:18.048877', NULL, NULL),
(3, NULL, 'ALTA', 'Usuario', 2, '2026-11-04 03:26:39.407201', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/689b4dbbf5a2470c9d4a6964329e1ab8.jpg\",\"CreatedAt\":\"2026-11-04T03:26:39.4067887Z\",\"Email\":\"sebascastro@ulp.edu.ar\",\"Nombre\":\"Sebas Castro\",\"Rol\":\"Usuario\"}'),
(4, NULL, 'ALTA', 'Usuario', 3, '2026-11-04 03:35:47.915238', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/270f4674a8464fbc8cf7e9570a52cba1.jpg\",\"CreatedAt\":\"2026-11-04T03:35:47.9148606Z\",\"Email\":\"florcastro@ulp.edu.ar\",\"Nombre\":\"Flor Castro\",\"Rol\":\"Usuario\"}'),
(5, NULL, 'ALTA', 'Usuario', 4, '2026-11-04 03:37:47.401436', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/cd1d16574f7845159e503e79947e9bf6.jpg\",\"CreatedAt\":\"2026-11-04T03:37:47.40123Z\",\"Email\":\"grupobicastro@ulp.edu.ar\",\"Nombre\":\"Grupo (bi)Castro\",\"Rol\":\"Usuario\"}'),
(6, NULL, 'ALTA', 'Usuario', 5, '2026-11-04 03:38:07.321250', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/b6871c08ec2f414eb2601d7f407f95aa.jpg\",\"CreatedAt\":\"2026-11-04T03:38:07.3209807Z\",\"Email\":\"inmobiliariacastro@ulp.edu.ar\",\"Nombre\":\"Inmobiliaria Castro\",\"Rol\":\"Usuario\"}'),
(7, NULL, 'ALTA', 'Usuario', 6, '2026-11-04 03:38:52.271840', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/e387cdf768064116b00b7925f81358ab.jpg\",\"CreatedAt\":\"2026-11-04T03:38:52.2716726Z\",\"Email\":\"tuamoresazulcastro@ulp.edu.ar\",\"Nombre\":\"Cristian Castro\",\"Rol\":\"Usuario\"}'),
(8, NULL, 'ALTA', 'Usuario', 7, '2026-11-04 03:39:46.829457', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/84c031e10f5d46f696876f8964a15800.jpg\",\"CreatedAt\":\"2026-11-04T03:39:46.8291583Z\",\"Email\":\"deningunamaneracastro@ulp.edu.ar\",\"Nombre\":\"Nelson Castro\",\"Rol\":\"Usuario\"}'),
(9, NULL, 'ALTA', 'Usuario', 8, '2026-11-04 03:41:08.178469', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/2bcaaba684784d9fad464b60a3080fb0.jpg\",\"CreatedAt\":\"2026-11-04T03:41:08.178227Z\",\"Email\":\"quenoloabsuelvalahistoriacastro@ulp.edu.ar\",\"Nombre\":\"Fidel Castro\",\"Rol\":\"Usuario\"}'),
(10, NULL, 'ALTA', 'Usuario', 9, '2026-11-04 03:41:57.961671', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/bb9d90af47fd4840a4c2be2aae1a8210.jpg\",\"CreatedAt\":\"2026-11-04T03:41:57.9615121Z\",\"Email\":\"rolicastro@ulp.edu.ar\",\"Nombre\":\"Roli Castro\",\"Rol\":\"Usuario\"}'),
(11, NULL, 'ALTA', 'Usuario', 10, '2026-11-04 04:06:00.194931', NULL, '{\"Activo\":true,\"AvatarUrl\":\"/uploads/avatares/f7d5180b03c34809a082153192584330.jpg\",\"CreatedAt\":\"2026-11-04T04:06:00.19466Z\",\"Email\":\"holaguapacastro@ulp.edu.ar\",\"Nombre\":\"Luciano Castro\",\"Rol\":\"Usuario\"}'),
(12, 10, 'INICIO_SESION', 'Usuario', 10, '2026-11-04 04:06:48.937205', NULL, NULL),
(13, 10, 'CIERRE_SESION', 'Usuario', 10, '2026-11-04 05:19:28.081792', NULL, NULL),
(14, 1, 'INICIO_SESION', 'Usuario', 1, '2026-11-04 05:41:21.257135', NULL, NULL),
(15, 1, 'CIERRE_SESION', 'Usuario', 1, '2026-11-04 05:43:08.173551', NULL, NULL),
(16, 10, 'INICIO_SESION', 'Usuario', 10, '2026-11-04 05:56:50.816148', NULL, NULL),
(17, 10, 'ALTA', 'Libro', 4, '2026-11-04 06:19:10.874520', NULL, '{\"Aclaracion\":\"Lo le\\u00ED 20 veces, pero lo cuid\\u00E9 mucho, est\\u00E1 en muy buen estado\",\"Activo\":true,\"Autor\":\"Alba Cardalda\",\"CategoriaId\":15,\"CreatedAt\":\"2026-11-04T03:19:10.8198527-03:00\",\"Disponible\":true,\"Editorial\":\"Vergara\",\"EstadoConservacion\":\"UsadoMuyBueno\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/a0b8ee6c39754868a6f38cd003e54f9d.jpg\",\"Titulo\":\"C\\u00F3mo dejar de ser tu peor enemigo\",\"UsuarioId\":10}'),
(18, 10, 'ALTA', 'Libro', 5, '2026-11-04 06:21:22.205571', NULL, '{\"Aclaracion\":\"es la impresion de una novela de wattpad con la que me senti muy identificado\",\"Activo\":true,\"Autor\":\"@BrIFC03\",\"CategoriaId\":13,\"CreatedAt\":\"2026-11-04T03:21:22.2052239-03:00\",\"Disponible\":true,\"Editorial\":\"Independiente / Wattpad (Digital)\",\"EstadoConservacion\":\"UsadoMediocre\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/eed4472ab94b4c15a96f0b7ec54bd2a5.png\",\"Titulo\":\"No creas que solo soy una cara bonita...\",\"UsuarioId\":10}'),
(19, 10, 'ALTA', 'Libro', 6, '2026-11-04 06:22:21.669269', NULL, '{\"Aclaracion\":\"No lo recomiendo mucho, pero si alguien lo quiere y me lo quiere cambiar por una fotocopia de jardineria acepto\",\"Activo\":true,\"Autor\":\"Los Indispensables\",\"CategoriaId\":9,\"CreatedAt\":\"2026-11-04T03:22:21.6689885-03:00\",\"Disponible\":true,\"Editorial\":\"Los Indispensables Ediciones\",\"EstadoConservacion\":\"UsadoMediocre\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/a49db73234f64ef2b0182ebd8c872f27.png\",\"Titulo\":\"C\\u00F3mo hacer dinero sin encuerarte\",\"UsuarioId\":10}'),
(20, 10, 'ALTA', 'Libro', 7, '2026-11-04 06:23:16.933437', NULL, '{\"Aclaracion\":\"Ganas de revolearlo\",\"Activo\":true,\"Autor\":\"M\\u00F3nica Garc\\u00EDa-Vi\\u00F1\\u00F3 S\\u00E1nchez y M\\u00AA \\u00C1ngeles Ortiz G\\u00F3mez\",\"CategoriaId\":10,\"CreatedAt\":\"2026-11-04T03:23:16.9331698-03:00\",\"Disponible\":true,\"Editorial\":\"Acento Espa\\u00F1ol, S.L.\",\"EstadoConservacion\":\"UsadoMalo\",\"Formato\":\"TapaDura\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/7b1b70f12111422ba15f1ac7829cd36a.png\",\"Titulo\":\"Acento espa\\u00F1ol: Curso de espa\\u00F1ol para argentinos\",\"UsuarioId\":10}'),
(21, 10, 'ALTA', 'Libro', 8, '2026-11-04 06:25:01.589416', NULL, '{\"Aclaracion\":\"interesante\",\"Activo\":true,\"Autor\":\"T. Navarro Tom\\u00E1s\",\"CategoriaId\":10,\"CreatedAt\":\"2026-11-04T03:25:01.5890129-03:00\",\"Disponible\":true,\"Editorial\":\"Academia espa\\u00F1ola\",\"EstadoConservacion\":\"UsadoMalo\",\"Formato\":\"TapaDura\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/f9bc01e6137f4fccbf62027f39bc92fc.jpg\",\"Titulo\":\"El acento castellano\",\"UsuarioId\":10}'),
(22, 10, 'CIERRE_SESION', 'Usuario', 10, '2026-11-04 06:25:30.212436', NULL, NULL),
(23, 10, 'INICIO_SESION', 'Usuario', 10, '2026-11-04 06:29:36.053529', NULL, NULL),
(24, 10, 'CIERRE_SESION', 'Usuario', 10, '2026-11-04 06:33:42.992726', NULL, NULL),
(25, 3, 'INICIO_SESION', 'Usuario', 3, '2026-11-04 06:34:03.518625', NULL, NULL),
(26, 3, 'ALTA', 'Libro', 9, '2026-11-04 06:34:56.455842', NULL, '{\"Aclaracion\":\"Tiene un leve doblez en la esquina inferior de la contratapa, pero las p\\u00E1ginas est\\u00E1n impecables y sin marcas.\",\"Activo\":true,\"Autor\":\"Patrick Rothfuss\",\"CategoriaId\":7,\"CreatedAt\":\"2026-11-04T03:34:56.4556136-03:00\",\"Disponible\":true,\"Editorial\":\"Debolsillo\",\"EstadoConservacion\":\"UsadoMuyBueno\",\"Formato\":\"TapaDura\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/c13c14aeb80f438d86ee18e628c2199f.png\",\"Titulo\":\"El nombre del viento\",\"UsuarioId\":3}'),
(27, 3, 'ALTA', 'Libro', 10, '2026-11-04 06:35:38.320957', NULL, '{\"Aclaracion\":\"Ejemplar directo de librer\\u00EDa, sin uso y conservado en estanter\\u00EDa cerrada.\",\"Activo\":true,\"Autor\":\"Carlos Ruiz Zaf\\u00F3n\",\"CategoriaId\":11,\"CreatedAt\":\"2026-11-04T03:35:38.3207599-03:00\",\"Disponible\":true,\"Editorial\":\"Planeta\",\"EstadoConservacion\":\"Nuevo\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/9b6ff3007f934946b1cd3fd7fe579a13.jpg\",\"Titulo\":\"La sombra del viento\",\"UsuarioId\":3}'),
(28, 3, 'ALTA', 'Libro', 11, '2026-11-04 06:36:15.940245', NULL, '{\"Aclaracion\":\"El lomo presenta desgaste visible por lectura frecuente y las primeras p\\u00E1ginas tienen una peque\\u00F1a mancha de humedad en el margen, pero est\\u00E1 completo y legible.\",\"Activo\":true,\"Autor\":\"Brandon Sanderson\",\"CategoriaId\":7,\"CreatedAt\":\"2026-11-04T03:36:15.9400429-03:00\",\"Disponible\":true,\"Editorial\":\"Nova\",\"EstadoConservacion\":\"UsadoMediocre\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/3cee7917dd464ce8a84d15a05f1d2277.png\",\"Titulo\":\"Nacidos de la bruma\",\"UsuarioId\":3}'),
(29, 3, 'CIERRE_SESION', 'Usuario', 3, '2026-11-04 06:42:05.548677', NULL, NULL),
(30, 8, 'INICIO_SESION', 'Usuario', 8, '2026-11-04 06:42:48.617064', NULL, NULL),
(31, 8, 'ALTA', 'Libro', 12, '2026-11-04 06:46:54.399628', NULL, '{\"Aclaracion\":\"Papel amarillento por el paso del tiempo, lomo gastado y olor a habano viejo impregnado en las hojas.\",\"Activo\":true,\"Autor\":\"Fidel Castro\",\"CategoriaId\":12,\"CreatedAt\":\"2026-11-04T03:46:54.3990903-03:00\",\"Disponible\":true,\"Editorial\":\"Ocean Sur\",\"EstadoConservacion\":\"UsadoMuyBueno\",\"Formato\":\"TapaDura\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/08571723110d42bcb84a98888632d945.png\",\"Titulo\":\"La historia me absolver\\u00E1\",\"UsuarioId\":8}'),
(32, 8, 'ALTA', 'Libro', 13, '2026-11-04 06:48:06.286838', NULL, '{\"Aclaracion\":\"Subrayado intensamente con marcador rojo hasta la mitad; la segunda mitad qued\\u00F3 virgen por falta de paciencia con el autor franc\\u00E9s.\",\"Activo\":true,\"Autor\":\"Thomas Piketty\",\"CategoriaId\":17,\"CreatedAt\":\"2026-11-04T03:48:06.2866456-03:00\",\"Disponible\":true,\"Editorial\":\"Fondo de Cultura Econ\\u00F3mica\",\"EstadoConservacion\":\"UsadoMediocre\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/ad3f65189ada472cbbfe02d9243c2fc6.png\",\"Titulo\":\"Capital en el siglo XXI\",\"UsuarioId\":8}'),
(33, 8, 'ALTA', 'Libro', 14, '2026-11-04 06:49:00.321545', NULL, '{\"Aclaracion\":\"Dedicatoria manuscrita y firmada en la primera p\\u00E1gina por el mism\\u00EDsimo Gabo en su visita a La Habana.\",\"Activo\":true,\"Autor\":\"Gabriel Garc\\u00EDa M\\u00E1rquez\",\"CategoriaId\":7,\"CreatedAt\":\"2026-11-04T03:49:00.3213622-03:00\",\"Disponible\":true,\"Editorial\":\"Debolsillo\",\"EstadoConservacion\":\"UsadoMuyBueno\",\"Formato\":\"TapaBlanda\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/e273163908e64a028b0f06f349f36465.png\",\"Titulo\":\"Cien a\\u00F1os de soledad\",\"UsuarioId\":8}'),
(34, 8, 'ALTA', 'Libro', 15, '2026-11-04 06:49:51.965386', NULL, '{\"Aclaracion\":\"Ejemplar de regalo protocolar recibido en una cumbre, con las p\\u00E1ginas totalmente limpias y sin abrir.\",\"Activo\":true,\"Autor\":\"Andr\\u00E9s Oppenheimer\",\"CategoriaId\":1,\"CreatedAt\":\"2026-11-04T03:49:51.964938-03:00\",\"Disponible\":true,\"Editorial\":\"Debate\",\"EstadoConservacion\":\"Nuevo\",\"Formato\":\"TapaDura\",\"Idioma\":\"Espa\\u00F1ol\",\"PortadaUrl\":\"/uploads/portadas/757b85d27b2a4b199d6c67ea3e559387.png\",\"Titulo\":\"S\\u00E1lvese quien pueda: El futuro del trabajo en la era de la automatizaci\\u00F3n\",\"UsuarioId\":8}'),
(35, 8, 'ALTA', 'Intercambio', 2, '2026-11-04 06:52:12.730065', NULL, '{\"Activo\":true,\"CreatedAt\":\"2026-11-04T03:52:12.7060908-03:00\",\"Estado\":\"Pendiente\",\"LibroOfrecidoId\":13,\"LibroSolicitadoId\":7,\"UsuarioDestinoId\":10,\"UsuarioOrigenId\":8}'),
(36, 8, 'ALTA', 'Mensaje', 1, '2026-11-04 06:52:32.645297', NULL, '{\"Contenido\":\"Hola guapo\",\"FechaHora\":\"2026-11-04T03:52:32.6321032-03:00\",\"IntercambioId\":2,\"UsuarioEmisorId\":8}'),
(37, 8, 'CIERRE_SESION', 'Usuario', 8, '2026-11-04 06:53:42.694585', NULL, NULL),
(38, 3, 'INICIO_SESION', 'Usuario', 3, '2026-11-04 06:53:48.527628', NULL, NULL),
(39, 3, 'ALTA', 'Intercambio', 3, '2026-11-04 06:54:43.732417', NULL, '{\"Activo\":true,\"CreatedAt\":\"2026-11-04T03:54:43.7321184-03:00\",\"Estado\":\"Pendiente\",\"LibroOfrecidoId\":9,\"LibroSolicitadoId\":14,\"UsuarioDestinoId\":8,\"UsuarioOrigenId\":3}'),
(40, 3, 'ALTA', 'Mensaje', 2, '2026-11-04 06:55:24.182697', NULL, '{\"Contenido\":\"Podr\\u00EDas enviarme una foto al 24564564456456456 por favor? tengo whatsapp\",\"FechaHora\":\"2026-11-04T03:55:24.1825736-03:00\",\"IntercambioId\":3,\"UsuarioEmisorId\":3}'),
(41, 3, 'CAMBIO_ESTADO_INTERCAMBIO', 'Intercambio', 3, '2026-11-04 06:58:16.720094', '{\"Estado\":\"Pendiente\"}', '{\"Estado\":\"Cancelado\"}'),
(42, 3, 'CIERRE_SESION', 'Usuario', 3, '2026-11-04 06:59:36.883693', NULL, NULL),
(43, 10, 'INICIO_SESION', 'Usuario', 10, '2026-11-04 06:59:44.412425', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `categoria`
--

CREATE TABLE `categoria` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `categoria`
--

INSERT INTO `categoria` (`id`, `nombre`, `activo`) VALUES
(1, 'Tecnología y Computación', 1),
(2, 'Moda y Diseño', 1),
(3, 'Gastronomía y Hogar', 1),
(4, 'Revistas y Periódicos', 1),
(5, 'Deportes y Bienestar', 1),
(6, 'Ciencia Ficción', 1),
(7, 'Fantasía', 1),
(8, 'Humor', 1),
(9, 'Autoayuda y Superación', 1),
(10, 'Idiomas y Lingüística', 1),
(11, 'Novela Negra y Policial', 1),
(12, 'Historia y Biografías', 1),
(13, 'Infantil y Juvenil', 1),
(14, 'Ciencia y Naturaleza', 1),
(15, 'Arte y Fotografía', 1),
(16, 'Poesía y Teatro', 1),
(17, 'Filosofía y Religión', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `intercambio`
--

CREATE TABLE `intercambio` (
  `id` int(11) NOT NULL,
  `usuario_origen_id` int(11) NOT NULL,
  `usuario_destino_id` int(11) NOT NULL,
  `libro_ofrecido_id` int(11) NOT NULL,
  `libro_solicitado_id` int(11) NOT NULL,
  `estado` enum('Pendiente','Aceptado','Rechazado','Exitoso','Cancelado') NOT NULL DEFAULT 'Pendiente',
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `intercambio`
--

INSERT INTO `intercambio` (`id`, `usuario_origen_id`, `usuario_destino_id`, `libro_ofrecido_id`, `libro_solicitado_id`, `estado`, `activo`, `created_at`) VALUES
(2, 8, 10, 13, 7, 'Pendiente', 1, '2026-11-04 06:52:12'),
(3, 3, 8, 9, 14, 'Cancelado', 1, '2026-11-04 06:54:43');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `libro`
--

CREATE TABLE `libro` (
  `id` int(11) NOT NULL,
  `usuario_id` int(11) NOT NULL,
  `categoria_id` int(11) NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `autor` varchar(255) NOT NULL,
  `editorial` varchar(255) NOT NULL,
  `idioma` varchar(255) NOT NULL,
  `formato` enum('Tapa dura','Tapa blanda','Digital') DEFAULT NULL,
  `aclaracion` varchar(255) DEFAULT NULL,
  `portada_url` varchar(255) NOT NULL,
  `estado_conservacion` enum('Nuevo','Usado','UsadoMediocre','UsadoMuyBueno','UsadoMalo') DEFAULT NULL,
  `disponible` tinyint(1) NOT NULL DEFAULT 1,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `libro`
--

INSERT INTO `libro` (`id`, `usuario_id`, `categoria_id`, `titulo`, `autor`, `editorial`, `idioma`, `formato`, `aclaracion`, `portada_url`, `estado_conservacion`, `disponible`, `activo`, `created_at`) VALUES
(4, 10, 15, 'Cómo dejar de ser tu peor enemigo', 'Alba Cardalda', 'Vergara', 'Español', 'Tapa blanda', 'Lo leí 20 veces, pero lo cuidé mucho, está en muy buen estado', '/uploads/portadas/a0b8ee6c39754868a6f38cd003e54f9d.jpg', 'UsadoMuyBueno', 1, 1, '2026-11-04 06:19:10'),
(5, 10, 13, 'No creas que solo soy una cara bonita...', '@BrIFC03', 'Independiente / Wattpad (Digital)', 'Español', 'Tapa blanda', 'es la impresion de una novela de wattpad con la que me senti muy identificado', '/uploads/portadas/eed4472ab94b4c15a96f0b7ec54bd2a5.png', 'UsadoMediocre', 1, 1, '2026-11-04 06:21:22'),
(6, 10, 9, 'Cómo hacer dinero sin encuerarte', 'Los Indispensables', 'Los Indispensables Ediciones', 'Español', 'Tapa blanda', 'No lo recomiendo mucho, pero si alguien lo quiere y me lo quiere cambiar por una fotocopia de jardineria acepto', '/uploads/portadas/a49db73234f64ef2b0182ebd8c872f27.png', 'UsadoMediocre', 1, 1, '2026-11-04 06:22:21'),
(7, 10, 10, 'Acento español: Curso de español para argentinos', 'Mónica García-Viñó Sánchez y Mª Ángeles Ortiz Gómez', 'Acento Español, S.L.', 'Español', 'Tapa dura', 'Ganas de revolearlo', '/uploads/portadas/7b1b70f12111422ba15f1ac7829cd36a.png', 'UsadoMalo', 1, 1, '2026-11-04 06:23:16'),
(8, 10, 10, 'El acento castellano', 'T. Navarro Tomás', 'Academia española', 'Español', 'Tapa dura', 'interesante', '/uploads/portadas/f9bc01e6137f4fccbf62027f39bc92fc.jpg', 'UsadoMalo', 1, 1, '2026-11-04 06:25:01'),
(9, 3, 7, 'El nombre del viento', 'Patrick Rothfuss', 'Debolsillo', 'Español', 'Tapa dura', 'Tiene un leve doblez en la esquina inferior de la contratapa, pero las páginas están impecables y sin marcas.', '/uploads/portadas/c13c14aeb80f438d86ee18e628c2199f.png', 'UsadoMuyBueno', 1, 1, '2026-11-04 06:34:56'),
(10, 3, 11, 'La sombra del viento', 'Carlos Ruiz Zafón', 'Planeta', 'Español', 'Tapa blanda', 'Ejemplar directo de librería, sin uso y conservado en estantería cerrada.', '/uploads/portadas/9b6ff3007f934946b1cd3fd7fe579a13.jpg', 'Nuevo', 1, 1, '2026-11-04 06:35:38'),
(11, 3, 7, 'Nacidos de la bruma', 'Brandon Sanderson', 'Nova', 'Español', 'Tapa blanda', 'El lomo presenta desgaste visible por lectura frecuente y las primeras páginas tienen una pequeña mancha de humedad en el margen, pero está completo y legible.', '/uploads/portadas/3cee7917dd464ce8a84d15a05f1d2277.png', 'UsadoMediocre', 1, 1, '2026-11-04 06:36:15'),
(12, 8, 12, 'La historia me absolverá', 'Fidel Castro', 'Ocean Sur', 'Español', 'Tapa dura', 'Papel amarillento por el paso del tiempo, lomo gastado y olor a habano viejo impregnado en las hojas.', '/uploads/portadas/08571723110d42bcb84a98888632d945.png', 'UsadoMuyBueno', 1, 1, '2026-11-04 06:46:54'),
(13, 8, 17, 'Capital en el siglo XXI', 'Thomas Piketty', 'Fondo de Cultura Económica', 'Español', 'Tapa blanda', 'Subrayado intensamente con marcador rojo hasta la mitad; la segunda mitad quedó virgen por falta de paciencia con el autor francés.', '/uploads/portadas/ad3f65189ada472cbbfe02d9243c2fc6.png', 'UsadoMediocre', 1, 1, '2026-11-04 06:48:06'),
(14, 8, 7, 'Cien años de soledad', 'Gabriel García Márquez', 'Debolsillo', 'Español', 'Tapa blanda', 'Dedicatoria manuscrita y firmada en la primera página por el mismísimo Gabo en su visita a La Habana.', '/uploads/portadas/e273163908e64a028b0f06f349f36465.png', 'UsadoMuyBueno', 1, 1, '2026-11-04 06:49:00'),
(15, 8, 1, 'Sálvese quien pueda: El futuro del trabajo en la era de la automatización', 'Andrés Oppenheimer', 'Debate', 'Español', 'Tapa dura', 'Ejemplar de regalo protocolar recibido en una cumbre, con las páginas totalmente limpias y sin abrir.', '/uploads/portadas/757b85d27b2a4b199d6c67ea3e559387.png', 'Nuevo', 1, 1, '2026-11-04 06:49:51');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mensaje`
--

CREATE TABLE `mensaje` (
  `id` int(11) NOT NULL,
  `intercambio_id` int(11) NOT NULL,
  `usuario_emisor_id` int(11) NOT NULL,
  `contenido` text NOT NULL,
  `fecha_hora` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `mensaje`
--

INSERT INTO `mensaje` (`id`, `intercambio_id`, `usuario_emisor_id`, `contenido`, `fecha_hora`) VALUES
(1, 2, 8, 'Hola guapo', '2026-11-04 06:52:32'),
(2, 3, 3, 'Podrías enviarme una foto al 24564564456456456 por favor? tengo whatsapp', '2026-11-04 06:55:24');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `password_hash` varchar(255) DEFAULT NULL,
  `rol` enum('Administrador','Usuario') NOT NULL DEFAULT 'Usuario',
  `avatar_url` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`id`, `nombre`, `email`, `password_hash`, `rol`, `avatar_url`, `activo`, `created_at`) VALUES
(1, 'Eri Castro', 'ericastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEKwYCEGOr45Vs/C7WixvJI+CRehoy9UZpjbDf2YxfG7LVwIwpjk3pJTAYAgv8EaFQg==', 'Administrador', '/uploads/avatares/06d3b7503f0e4a47a45b34905a59034c.jpg', 1, '2026-11-04 06:26:06'),
(2, 'Sebas Castro', 'sebascastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAENQf4xtDwdQ+Op3idOKfvU7ytXHAjoqNkb0+hlC2bV1/XjSNZbwnVd9IywpiBLoErw==', 'Administrador', '/uploads/avatares/689b4dbbf5a2470c9d4a6964329e1ab8.jpg', 1, '2026-11-04 06:26:39'),
(3, 'Flor Castro', 'florcastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEAj/+EivS3q9+K6QeUwLXCSKD70O45G/DffgF3SaR0QpurLRLZ6/zPgpx+WH1h+MvQ==', 'Usuario', '/uploads/avatares/270f4674a8464fbc8cf7e9570a52cba1.jpg', 1, '2026-11-04 06:35:47'),
(4, 'Grupo (bi)Castro', 'grupobicastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEP2Ecb3m5WMIyExG9z5cilD/u3TxUMmLZ7crcHGrLv4PwRzN8sTGGuQieeh5MMBg/A==', 'Usuario', '/uploads/avatares/cd1d16574f7845159e503e79947e9bf6.jpg', 1, '2026-11-04 06:37:47'),
(5, 'Inmobiliaria Castro', 'inmobiliariacastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEIzDp08V0sJWsoI51iEPxxLgxIxl+0ZxnjcjByzlndgRIentRzw+oUKQx/mqZEDC8A==', 'Usuario', '/uploads/avatares/b6871c08ec2f414eb2601d7f407f95aa.jpg', 1, '2026-11-04 06:38:07'),
(6, 'Cristian Castro', 'tuamoresazulcastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEBg4p2LBS0GuSqu8y+T2lN3ugB2svskREgLgt8kLfOpmmj10f7G4TkDlwbnaHKfHZw==', 'Usuario', '/uploads/avatares/e387cdf768064116b00b7925f81358ab.jpg', 1, '2026-11-04 06:38:52'),
(7, 'Nelson Castro', 'deningunamaneracastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEEZ/Bi0c7tWlpihcKjnZ/2iEhTRL7aRrohWh0xLMxM7aNHE1R1hWrv23zzvxf1+kCw==', 'Usuario', '/uploads/avatares/84c031e10f5d46f696876f8964a15800.jpg', 1, '2026-11-04 06:39:46'),
(8, 'Fidel Castro', 'quenoloabsuelvalahistoriacastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEHYnt1LxiG0FWYvecf0I4M2tDMfqA2dWLob76yvj9cGclr7KBWlqQ0VCsrAq6hIIFw==', 'Usuario', '/uploads/avatares/2bcaaba684784d9fad464b60a3080fb0.jpg', 1, '2026-11-04 06:41:08'),
(9, 'Roli Castro', 'rolicastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEHVXsLNTgkAKJSKlFMoA9xu9+byRwH5dcK6O3Wj/oNbYDHxNklPAT6tsZouyR3gb/w==', 'Usuario', '/uploads/avatares/bb9d90af47fd4840a4c2be2aae1a8210.jpg', 1, '2026-11-04 06:41:57'),
(10, 'Luciano Castro', 'holaguapacastro@ulp.edu.ar', 'AQAAAAIAAYagAAAAEPQ3Plm+G+BecLHBhv4TOsCmogPNz1Z9BBR60zQmv/aSsxlM2czyzuVXQmrSaoVIlA==', 'Usuario', '/uploads/avatares/f7d5180b03c34809a082153192584330.jpg', 1, '2026-11-04 07:06:00');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  ADD PRIMARY KEY (`id_auditoria`);

--
-- Indices de la tabla `categoria`
--
ALTER TABLE `categoria`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `intercambio`
--
ALTER TABLE `intercambio`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_intercambio_usuario_origen` (`usuario_origen_id`),
  ADD KEY `fk_intercambio_usuario_destino` (`usuario_destino_id`),
  ADD KEY `fk_intercambio_libro_ofrecido` (`libro_ofrecido_id`),
  ADD KEY `fk_intercambio_libro_solicitado` (`libro_solicitado_id`);

--
-- Indices de la tabla `libro`
--
ALTER TABLE `libro`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_libro_usuario` (`usuario_id`),
  ADD KEY `fk_libro_categoria` (`categoria_id`);

--
-- Indices de la tabla `mensaje`
--
ALTER TABLE `mensaje`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_mensaje_intercambio` (`intercambio_id`),
  ADD KEY `fk_mensaje_usuario_emisor` (`usuario_emisor_id`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  MODIFY `id_auditoria` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=44;

--
-- AUTO_INCREMENT de la tabla `categoria`
--
ALTER TABLE `categoria`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT de la tabla `intercambio`
--
ALTER TABLE `intercambio`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `libro`
--
ALTER TABLE `libro`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `mensaje`
--
ALTER TABLE `mensaje`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `intercambio`
--
ALTER TABLE `intercambio`
  ADD CONSTRAINT `fk_intercambio_libro_ofrecido` FOREIGN KEY (`libro_ofrecido_id`) REFERENCES `libro` (`id`),
  ADD CONSTRAINT `fk_intercambio_libro_solicitado` FOREIGN KEY (`libro_solicitado_id`) REFERENCES `libro` (`id`),
  ADD CONSTRAINT `fk_intercambio_usuario_destino` FOREIGN KEY (`usuario_destino_id`) REFERENCES `usuario` (`id`),
  ADD CONSTRAINT `fk_intercambio_usuario_origen` FOREIGN KEY (`usuario_origen_id`) REFERENCES `usuario` (`id`);

--
-- Filtros para la tabla `libro`
--
ALTER TABLE `libro`
  ADD CONSTRAINT `fk_libro_categoria` FOREIGN KEY (`categoria_id`) REFERENCES `categoria` (`id`),
  ADD CONSTRAINT `fk_libro_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`);

--
-- Filtros para la tabla `mensaje`
--
ALTER TABLE `mensaje`
  ADD CONSTRAINT `fk_mensaje_intercambio` FOREIGN KEY (`intercambio_id`) REFERENCES `intercambio` (`id`),
  ADD CONSTRAINT `fk_mensaje_usuario_emisor` FOREIGN KEY (`usuario_emisor_id`) REFERENCES `usuario` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
