SISTEMA DE GESTIÓN SAKILAAPP - ASP.NET CORE MVC
DESCRIPCIÓN GENERAL

Este proyecto consiste en una aplicación web empresarial desarrollada como práctica de laboratorio para la asignatura de Desarrollo Web para Integración de Tecnologías. El sistema permite la gestión integral de la base de datos Sakila, aplicando una arquitectura moderna y buenas prácticas de desarrollo de software orientadas a sistemas escalables, seguros y mantenibles.

CARACTERÍSTICAS PRINCIPALES (6 ETAPAS)

1. Arquitectura MVC:
Se implementa el patrón Modelo–Vista–Controlador, permitiendo una separación clara de responsabilidades para lograr un código más organizado y escalable en ASP.NET Core MVC.

2. Conectividad con EF Core:
Uso de Entity Framework Core bajo enfoque Database-First para la persistencia y gestión de datos en SQL Server 2022.

3. Seguridad y autenticación:
Implementación de ASP.NET Core Identity para el registro, inicio de sesión y control seguro de usuarios.

4. Eliminación lógica (Soft Delete):
Los registros no se eliminan físicamente, sino que se marcan como inactivos, preservando el historial y la integridad de los datos.

5. Filtrado y paginación:
Uso de consultas LINQ para mostrar únicamente registros activos, mejorando el rendimiento y la experiencia del usuario.

6. Dashboard operativo:
Panel de control con métricas en tiempo real mediante consultas asíncronas para el análisis de indicadores clave (KPI).

TECNOLOGÍAS UTILIZADAS
Lenguaje: C# (.NET 10.0)
Framework: ASP.NET Core MVC
ORM: Entity Framework Core
Base de datos: SQL Server 2022
Frontend: Bootstrap 5 y jQuery
INSTRUCCIONES DE CONFIGURACIÓN
Requisitos previos
Visual Studio 2026
SQL Server 2022
Base de datos Sakila restaurada
Instalación

1. Clonar el repositorio:

git clone https://github.com/BruceRodri/P1Lab2AplicacionWeb-Rodriguez_Bruce.git

2. Configurar cadena de conexión:
Modificar el archivo appsettings.json con las credenciales locales de SQL Server.

3. Ejecutar migraciones (opcional):

Update-Database

4. Iniciar la aplicación:
Presionar F5 en Visual Studio para ejecutar el servidor local.
