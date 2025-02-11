# EVALUACIÓN TÉCNICA NUXIBA

Prueba: **DESARROLLADOR JR**

Deadline: **1 día**

Nombre: 

Juan Jose Medina Montes 

## Clona y crea tu repositorio para la evaluación

1. Clona este repositorio en tu máquina local.
2. Crea un repositorio público en tu cuenta personal de GitHub, BitBucket o Gitlab.
3. Cambia el origen remoto para que apunte al repositorio público que acabas de crear en tu cuenta.
4. Coloca tu nombre en este archivo README.md y realiza un push al repositorio remoto.

---

## Instrucciones Generales

1. Cada pregunta tiene un valor asignado. Asegúrate de explicar tus respuestas y mostrar las consultas o procedimientos que utilizaste.
2. Se evaluará la claridad de las explicaciones, el pensamiento crítico, y la eficiencia de las consultas.
3. Utiliza **SQL Server** para realizar todas las pruebas y asegúrate de que las consultas funcionen correctamente antes de entregar.
4. Justifica tu enfoque cuando encuentres una pregunta sin una única respuesta correcta.
5. Configura un Contenedor de **SQL Server con Docker** utilizando los siguientes pasos:

### Pasos para ejecutar el contenedor de SQL Server

Asegúrate de tener Docker instalado y corriendo en tu máquina. Luego, ejecuta el siguiente comando para levantar un contenedor con SQL Server:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd'    -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

6. Conéctate al servidor de SQL con cualquier herramienta como **SQL Server Management Studio** o **Azure Data Studio** utilizando las siguientes credenciales:
   - **Servidor**: localhost, puerto 1433
   - **Usuario**: sa
   - **Contraseña**: YourStrong!Passw0rd

---

# Examen Práctico para Desarrollador Junior en .NET 8 y SQL Server

**Tiempo estimado:** 1 día  
**Total de puntos:** 100

---

## Instrucciones Generales:

El examen está compuesto por tres ejercicios prácticos. Sigue las indicaciones en cada uno y asegúrate de entregar el código limpio y funcional.

Además, se proporciona un archivo **CCenterRIA.xlsx** para que te bases en la estructura de las tablas y datos proporcionados.

[Descargar archivo de ejemplo](CCenterRIA.xlsx)

---

## Ejercicio 1: API RESTful con ASP.NET Core y Entity Framework (40 puntos)

**Instrucciones:**  
Desarrolla una API RESTful con ASP.NET Core y Entity Framework que permita gestionar el acceso de usuarios.

Se crearon los siguientes endpoints para interactuar con la tabla ccloglogin:
GET /logins: Devuelve todos los registros de logins y logouts de la tabla ccloglogin.
POST /logins: Permite registrar un nuevo login/logout.
PUT /logins/{id}: Permite actualizar un registro de login/logout.
DELETE /logins/{id}: Elimina un registro de login/logout.

Cree el modelo Login basado en los datos de la tabla ccloglogin el cual se ve de la siguiente forma:

public class Login
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Extension { get; set; }
    public int TipoMov { get; set; }
    public DateTime Fecha { get; set; }
}

Para la base de datos
Utilice Entity Framework Core para crear la tabla en una base de datos SQL Server basada en el modelo Login. 
Aplique las migraciones para crear la tabla en la base de datos.
Instale los paquetes NuGet necesarios para utilizar Entity Framework Core con SQL Server.
Cree el contexto de base de datos LoginContext que hereda de DbContext.
Configure la cadena de conexión en el archivo appsettings.json.
Y por ultimo aplique las migraciones para crear la tabla en la base de datos.

Para las validaciones
Implemente  validaciones necesarias para asegurar que las fechas sean válidas y que el UserId esté 
presente en la tabla ccUsers. Además, de manejar errores como intentar registrar un login sin un logout anterior.
Revise la validacio de las fechas para que sean válidas y no sean nulas.
Igual valide que el UserId esté presente en la tabla ccUsers.

En la conexion con SQL Server 
Utilice la cadena de conexión configurada en el archivo appsettings.json con los datos que corresponden a mi conexion.
Cree el contexto de base de datos LoginContext que hereda de DbContext.
---

## Ejercicio 2: Consultas SQL y Optimización (30 puntos)

**Instrucciones:**

Trabaja en SQL Server y realiza las siguientes consultas basadas en la tabla `ccloglogin`:


1. Consulta del usuario que más tiempo ha estado logueado
WITH Logueos AS (
  SELECT 
    UserId,
    TipoMov,
    Fecha,
    LAG(TipoMov) OVER (PARTITION BY UserId ORDER BY Fecha) AS Anterior
  FROM 
    Logins
)
, LogueosCalculados AS (
  SELECT 
    UserId,
    Fecha,
    LEAD(Fecha, 1) OVER (PARTITION BY UserId ORDER BY Fecha) AS FechaSiguiente
  FROM 
    Logueos
  WHERE 
    TipoMov = 1 AND (Anterior IS NULL OR Anterior = 0)
)
SELECT TOP 1 
  UserId,
  CONVERT(VARCHAR, DATEADD(DAY, SUM(DATEDIFF(DAY, Fecha, FechaSiguiente)), 0), 116) AS TiempoTotal
FROM 
  LogueosCalculados
GROUP BY 
  UserId
ORDER BY 
  SUM(DATEDIFF(DAY, Fecha, FechaSiguiente)) DESC;
/*Esta consulta crea una tabla temporal que se llama "Logueos" esta contiene la información de los logueos, que incluye ell UserId, el TipoMov haciendo referencia al tipo de movimiento (logueo = 1 o deslogueo = 0), la fecha del movimiento y la fecha del movimiento anterior para cada usuario. */
/*Después, se crea otra tabla temporal llamada "LogueosCalculados" esta se usa para seleccionar los logueos iniciales y calcula la fecha siguiente para cada uno de ellos. */
/*Por último, la consulta selecciona el usuario con el tiempo total de logueo más largo, sumando las diferencias entre las fechas de logueo y las fechas siguientes, y convirtiendo el resultado en una cadena que muestra el tiempo total en días, horas, minutos y segundos.*/

2. Consulta del usuario que menos tiempo ha estado logueado
WITH Logueos AS (
  SELECT 
    UserId,
    TipoMov,
    Fecha,
    LAG(TipoMov) OVER (PARTITION BY UserId ORDER BY Fecha) AS Anterior
  FROM 
    Logins
)
, LogueosCalculados AS (
  SELECT 
    UserId,
    Fecha,
    LEAD(Fecha, 1) OVER (PARTITION BY UserId ORDER BY Fecha) AS FechaSiguiente
  FROM 
    Logueos
  WHERE 
    TipoMov = 1 AND (Anterior IS NULL OR Anterior = 0)
)
SELECT TOP 1 
  UserId,
  CONVERT(VARCHAR, DATEADD(DAY, SUM(DATEDIFF(DAY, Fecha, FechaSiguiente)), 0), 116) AS TiempoTotal
FROM 
  LogueosCalculados
GROUP BY 
  UserId
ORDER BY 
  SUM(DATEDIFF(DAY, Fecha, FechaSiguiente)) ASC;

/*Partiendo de la consulta anterior y usando las tablas temporales se calcula la fecha siguiente para cada logueo inicial, después se suman las diferencias entre las fechas de logueo y las fechas siguientes, para así convertir el resultado en una cadena que muestra el tiempo total en días, horas, minutos y segundos utilizando el estilo de formato 116.*/
/*Finalmente ordena los resultados en orden ascendente para seleccionar el usuario con el tiempo total de logueo más corto, y devuelve solo el primer registro utilizando la cláusula SELECT TOP 1.*/

3. Promedio de logueo por mes
WITH Logueos AS (
  SELECT 
    UserId,
    TipoMov,
    Fecha,
    LAG(TipoMov) OVER (PARTITION BY UserId ORDER BY Fecha) AS Anterior
  FROM 
    Logins
)
, LogueosCalculados AS (
  SELECT 
    UserId,
    YEAR(Fecha) AS Ano,
    MONTH(Fecha) AS Mes,
    Fecha,
    LEAD(Fecha, 1) OVER (PARTITION BY UserId ORDER BY Fecha) AS FechaSiguiente
  FROM 
    Logueos
  WHERE 
    TipoMov = 1 AND (Anterior IS NULL OR Anterior = 0)
)
SELECT 
  UserId,
  Ano,
  Mes,
  Fecha,
  FechaSiguiente,
  CONVERT(VARCHAR, DATEADD(DAY, DATEDIFF(DAY, Fecha, FechaSiguiente), 0), 116) AS TiempoPromedio
FROM 
  LogueosCalculados
ORDER BY 
  UserId, Ano, Mes;

/*Como en las consultas anteriores se utilizan tablas temporales para calcular la fecha siguiente para cada logueo inicial, luego calcula la diferencia entre las fechas de logueo y las fechas siguientes, y finalmente muestra los resultados en una tabla que incluye el UserId, el año y mes del logueo, la fecha de logueo, la fecha siguiente y el tiempo que el usuario estuvo logueado, expresado en días, horas, minutos y segundos.*/


---

## Ejercicio 3: API RESTful para generación de CSV (30 puntos)

**Instrucciones:**

1. **Generación de CSV**:  
   Crea un endpoint adicional en tu API que permita generar un archivo CSV con los siguientes datos:
   - Nombre de usuario (`Login` de la tabla `ccUsers`)
   - Nombre completo (combinación de `Nombres`, `ApellidoPaterno`, y `ApellidoMaterno` de la tabla `ccUsers`)
   - Área (tomado de la tabla `ccRIACat_Areas`)
   - Total de horas trabajadas (basado en los registros de login y logout de la tabla `ccloglogin`)

   El CSV debe calcular el total de horas trabajadas por usuario sumando el tiempo entre logins y logouts.

2. **Formato y Entrega**:
   - El CSV debe ser descargable a través del endpoint de la API.
   - Asegúrate de probar este endpoint utilizando herramientas como **Postman** o **curl** y documenta los pasos en el archivo README.md.

---

## Entrega

1. Sube tu código a un repositorio en GitHub o Bitbucket y proporciona el enlace para revisión.
2. El repositorio debe contener las instrucciones necesarias en el archivo **README.md** para:
   - Levantar el contenedor de SQL Server.
   - Conectar la base de datos.
   - Ejecutar la API y sus endpoints.
   - Descargar el CSV generado.
3. **Opcional**: Si incluiste pruebas unitarias, indica en el README cómo ejecutarlas.

---

Este examen evalúa tu capacidad para desarrollar APIs RESTful, realizar consultas avanzadas en SQL Server y generar reportes en formato CSV. Se valorará la organización del código, las mejores prácticas y cualquier documentación adicional que proporciones.

Para la Generación de CSV
Cree un endpoint adicional en la API para generar el archivo CSV. El endpoint se llama GetCSV y se encuentra en el controlador LoginsController implementado de la siguiente forma:
[HttpGet]
[Route("csv")]
public async Task<IActionResult> GetCSV()
{
    var logins = await _context.Logins
        .Include(l => l.User)
        .Include(l => l.User.Area)
        .ToListAsync();

    var csv = new StringBuilder();
    csv.AppendLine("Nombre de usuario,Nombre completo,Área,Total de horas trabajadas");

    foreach (var login in logins)
    {
        if (login.User != null)
        {
            var user = login.User;
            var area = user.Area;
            var totalHoras = login.TipoMov == 1 ? (login.Fecha - login.Fecha).TotalHours : (login.Fecha - login.Fecha).TotalHours;

            csv.AppendLine($"{user.Login},{user.Nombres} {user.ApellidoPaterno} {user.ApellidoMaterno},{area.Nombre},{totalHoras}");
        }
    }

    return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "logins.csv");
}
El CSV generado se puede descargar a través del endpoint GetCSV. El archivo se llama logins.csv y se devuelve como un archivo de texto CSV.
Pruebas con Postman o curl
Se probaron los endpoints de la API utilizando Postman de esta forma:
1.	Abrir Postman y crear una nueva solicitud GET.
2.	Ingresamos la URL del endpoint GetCSV, por ejemplo, https://localhost:5001/api/logins/csv. El puerto en mi caso
3.	Hacemos clic en el botón "Enviar" para enviar la solicitud.
4.	El archivo CSV se descargará automáticamente.
