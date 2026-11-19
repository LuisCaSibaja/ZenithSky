-- Procedimientos almacenados de Airport y Airplane - ZenithSky
-- Ejecutar sobre la base ZenithSky_Proyecto_JEDL (después de crear las tablas)
-- Mismo estilo que los de Airline:
--   * Insertar  -> devuelve el id generado (SCOPE_IDENTITY)
--   * Actualizar/Eliminar -> devuelven las filas afectadas (@@ROWCOUNT)
--   * Existe/Tiene -> devuelven un conteo
USE ZenithSky_Proyecto_JEDL;
GO

/* =========================================================
   AIRPORT
   ========================================================= */

--Insertar aeropuerto
CREATE OR ALTER PROCEDURE sp_InsertarAirport
    @iata CHAR(3),
    @icao CHAR(4),
    @name VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO airport (iata, icao, name)
    VALUES (@iata, @icao, @name);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS airport_id;
END
GO

--Actualizar aeropuerto
CREATE OR ALTER PROCEDURE sp_ActualizarAirport
    @airport_id INT,
    @iata CHAR(3),
    @icao CHAR(4),
    @name VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE airport
    SET iata = @iata,
        icao = @icao,
        name = @name
    WHERE airport_id = @airport_id;

    SELECT @@ROWCOUNT AS rows_affected;
END
GO

--Eliminar aeropuerto
CREATE OR ALTER PROCEDURE sp_EliminarAirport
    @airport_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM airport
    WHERE airport_id = @airport_id;

    SELECT @@ROWCOUNT AS rows_affected;
END
GO

--Valida IATA duplicado (en el PUT se excluye el propio aeropuerto; en el POST se manda 0)
CREATE OR ALTER PROCEDURE sp_ExisteIataAirport
    @iata CHAR(3),
    @airport_id INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM airport
    WHERE iata = @iata
      AND airport_id <> @airport_id;
END
GO

--Valida ICAO duplicado (en el PUT se excluye el propio aeropuerto; en el POST se manda 0)
CREATE OR ALTER PROCEDURE sp_ExisteIcaoAirport
    @icao CHAR(4),
    @airport_id INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM airport
    WHERE icao = @icao
      AND airport_id <> @airport_id;
END
GO

--Para el 409 Conflict: cuántas aerolíneas tienen este aeropuerto como base
CREATE OR ALTER PROCEDURE sp_EsAeropuertoBase
    @airport_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM airline
    WHERE base_airport = @airport_id;
END
GO

/* =========================================================
   AIRPLANE
   ========================================================= */

--Listar aviones con su tipo, aerolínea y aeropuerto base (filtros opcionales ?typeId= y ?airlineId=)
CREATE OR ALTER PROCEDURE sp_ListarAirplane
    @type_id INT = NULL,
    @airline_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.airplane_id, a.capacity,
           t.type_id, t.identifier, t.description,
           al.airline_id, al.iata, al.airlinename,
           ap.airport_id, ap.iata, ap.icao, ap.name
    FROM airplane a
        JOIN airplane_type t ON a.type_id = t.type_id
        JOIN airline al ON a.airline_id = al.airline_id
        JOIN airport ap ON al.base_airport = ap.airport_id
    WHERE (@type_id IS NULL OR a.type_id = @type_id)
      AND (@airline_id IS NULL OR a.airline_id = @airline_id)
    ORDER BY a.airplane_id;
END
GO

--Obtener avión por id
CREATE OR ALTER PROCEDURE sp_ObtenerIdAirplane
    @airplane_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.airplane_id, a.capacity,
           t.type_id, t.identifier, t.description,
           al.airline_id, al.iata, al.airlinename,
           ap.airport_id, ap.iata, ap.icao, ap.name
    FROM airplane a
        JOIN airplane_type t ON a.type_id = t.type_id
        JOIN airline al ON a.airline_id = al.airline_id
        JOIN airport ap ON al.base_airport = ap.airport_id
    WHERE a.airplane_id = @airplane_id;
END
GO

--Insertar avión
CREATE OR ALTER PROCEDURE sp_InsertarAirplane
    @capacity INT,
    @type_id INT,
    @airline_id INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO airplane (capacity, type_id, airline_id)
    VALUES (@capacity, @type_id, @airline_id);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS airplane_id;
END
GO

--Actualizar avión
CREATE OR ALTER PROCEDURE sp_ActualizarAirplane
    @airplane_id INT,
    @capacity INT,
    @type_id INT,
    @airline_id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE airplane
    SET capacity = @capacity,
        type_id = @type_id,
        airline_id = @airline_id
    WHERE airplane_id = @airplane_id;

    SELECT @@ROWCOUNT AS rows_affected;
END
GO

--Eliminar avión
CREATE OR ALTER PROCEDURE sp_EliminarAirplane
    @airplane_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM airplane
    WHERE airplane_id = @airplane_id;

    SELECT @@ROWCOUNT AS rows_affected;
END
GO

--Valida que exista el tipo de avión (POST y PUT de airplane)
CREATE OR ALTER PROCEDURE sp_ExisteTipoAvion
    @type_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM airplane_type
    WHERE type_id = @type_id;
END
GO

--Valida que exista la aerolínea (POST y PUT de airplane)
CREATE OR ALTER PROCEDURE sp_ExisteAirline
    @airline_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM airline
    WHERE airline_id = @airline_id;
END
GO

/* Pruebas rápidas
EXEC sp_InsertarAirport 'LAX', 'KLAX', 'Aeropuerto Internacional de Los Ángeles';
EXEC sp_ActualizarAirport 5, 'LAX', 'KLAX', 'Los Angeles International Airport';
EXEC sp_EsAeropuertoBase 1;   -- > 0 => 409
EXEC sp_EsAeropuertoBase 4;   -- 0   => se puede eliminar
EXEC sp_ListarAirplane;
EXEC sp_ListarAirplane @type_id = 2;
EXEC sp_ListarAirplane @airline_id = 1;
EXEC sp_ObtenerIdAirplane 1;
*/
