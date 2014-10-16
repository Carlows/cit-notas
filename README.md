Centro de Innovación Tecnológica
=========

Lenguaje y Tecnologías que utilizará la aplicación

ASP.NET MVC 5 		- 	Framework  para el desarrollo de aplicaciones web
Entity Framework 6	- 	ORM Entidad-relación para controlar los datos
JQuery			-	Programacion para el cliente
Twitter Bootstrap	-	Diseño de la página web
LocalDB - SQL Server	-	Almacenamiento de datos

Patron de diseño MVC 

MODELO - Almacenamiento de los datos, operaciones que hagamos hacia la base de datos

VISTAS - Html, Jquery, CSS

CONTROLADORES - Darle un formato a los datos de nuestro modelo para que los consuman las vistas


------------------------------------------------------------------------------

Aplicación para el control de asistencias, notas, y minutas de los equipos de proyectos.

Tipos de usuario -> Coordinador de celula, Profesores.

- Inicialmente, la aplicación llevará el control de las minutas para los equipos de proyecto, 
te permitirá redactar la minuta desde el sistema y también subirlas como archivo de texto.
- Al profesor se le permite cambiar el status de los alumnos, tanto como a los coordinadores de celula,
como a los integrantes de cada una.
- Cuando a un alumno se le asigna el status de coordinador de celula, a este se le permite cargar minutas
para su celula. También podrá controlar la asistencia de los integrantes de su celula.
- Los profesores tendran una sección para controlar el calendario de actividades; las minutas podrán cargarse
de acuerdo a este calendario.
- Las minutas subidas por cada coordinador de celula necesitarán aprobación del profesor, por lo que este 
tendrá una sección para aprobar las minutas.
