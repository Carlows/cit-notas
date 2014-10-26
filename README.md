#Centro de Innovación Tecnológica
=========

###Lenguaje y Tecnologías que utilizará la aplicación

-**ASP.NET MVC 5** 		- 	Framework  para el desarrollo de aplicaciones web

-**Entity Framework 6**	- 	ORM Entidad-relación para controlar los datos

-**JQuery**			-	Programacion para el cliente

-**Twitter Bootstrap**	-	Diseño de la página web

-**LocalDB - SQL Server**	-	Almacenamiento de datos

Patron de diseño MVC 

MODELO - Almacenamiento de los datos, operaciones que hagamos hacia la base de datos

VISTAS - Html, Jquery, CSS

CONTROLADORES - Darle un formato a los datos de nuestro modelo para que los consuman las vistas


------------------------------------------------------------------------------

###Aplicación para el control de asistencias, notas, y minutas de los equipos de proyectos.

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

#### Diagrama inicial de la base de datos

http://prntscr.com/4xie2h

#### Pantalla de Log-in

http://prntscr.com/503ymm

#### Diseño inicial

http://prntscr.com/503ywh

----------------------------------------------------------------------------------

###TO-DOs

- Definir claramente las ideas principales de la aplicación (Manejo de notas, asistencia y minutas).


#### Done

- Modelamiento de la base de datos, definir las entidades y las relaciones.
- Diseño básico de la interfaz (podría ser mejorada luego).
- Controladores Administrador, Profesor, Coordinador, al usuario loguearse, este es redireccionado hacia el controlador segun su rol.
- Log-in.

----------------------------------------------------------------------------------

###Links de interes:

- Carpeta personal de dropbox con contenido sobre C#, Jquery, ASP.Net, LINQ, etc
  https://www.dropbox.com/sh/0dhg5tk897aqlrb/AACRkwcy_45ijSbVme1SE-Xua?dl=0
  (Recomiendo descargar PRO ASP.Net MVC 5, Learning JQuery y LINQ In Action)

- Sitio oficial de ASP.NET
  http://asp.net/
  (Recomiendo leer las secciones de ASP.NET MVC y Entity Framework) 

- Sitio de preguntas y respuestas sobre cualquier tema de programación
  http://stackoverflow.com/
  (Leer el FAQ para principiantes antes de hacer cualquier pregunta, por lo general ya otra persona ha tenido el mismo problema,   asi que antes de abrir una pregunta, busca muy bien dentro de la página.)
