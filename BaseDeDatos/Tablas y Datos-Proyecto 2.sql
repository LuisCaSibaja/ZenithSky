-- Base de datos ZenithSky 
--Jimena, Eilyn, Daniel y Luis
--Ver que nombre ponerle
USE master;
GO
--ZenithSky_Proyecto_JEDL
CREATE DATABASE ZenithSky_Proyecto_JEDL;

USE ZenithSky_Proyecto_JEDL;

--Aeropuerto
CREATE TABLE airport (
    airport_id INT IDENTITY(1,1) PRIMARY KEY,
    iata CHAR(3),
    icao CHAR(4),
    name VARCHAR(100)
);

--Aerolinea
CREATE TABLE airline (
    airline_id INT IDENTITY(1,1) PRIMARY KEY,
    iata CHAR(2),
    airlinename VARCHAR(100),
    base_airport INT,
    CONSTRAINT FK_airline_airport FOREIGN KEY (base_airport) REFERENCES airport(airport_id)
);

--Tipo de avión
CREATE TABLE airplane_type (
    type_id INT IDENTITY(1,1) PRIMARY KEY,
    identifier VARCHAR(50),
    description VARCHAR(250)
);

--Avión
CREATE TABLE airplane (
    airplane_id INT IDENTITY(1,1) PRIMARY KEY,
    capacity INT,
    type_id INT,
    airline_id INT,
    CONSTRAINT FK_airplane_airplane_type FOREIGN KEY (type_id) REFERENCES airplane_type(type_id),
    CONSTRAINT FK_airplane_airline FOREIGN KEY (airline_id) REFERENCES airline(airline_id)
);


--Datos de pruebas
--Incluye aeropuertos con aerolíneas asociadas y uno libre (id 4) para probar DELETE 200 vs 409 Conflict
INSERT INTO airport (iata, icao, name) VALUES
('SJO', 'MROC', 'Aeropuerto Internacional Juan Santamaría'),
('MIA', 'KMIA', 'Aeropuerto Internacional de Miami'),
('MAD', 'LEMD', 'Aeropuerto Adolfo Suárez Madrid-Barajas'),
('LIR', 'MRLB', 'Aeropuerto Internacional Daniel Oduber Quirós');
GO

--Aerolíneas 
--Incluye aerolíneas con aviones asignados y una libre (id 3) para probar DELETE 200 vs 409 Conflict
INSERT INTO airline (iata, airlinename, base_airport) VALUES
('ZS', 'ZenithSky Airlines', 1),
('AA', 'American Airlines', 2),
('IB', 'Iberia', 3);
GO

--Tipos de Avión
INSERT INTO airplane_type (identifier, description) VALUES
('B737-800', 'Boeing 737-800 de corto y medio alcance'),
('A320', 'Airbus A320 comercial de pasillo único'),
('E190', 'Embraer 190 regional'),
('B787-9', 'Boeing 787-9 Dreamliner de largo alcance');
GO

--Aviones (airplane_id autogenerados: 1 al 6)
INSERT INTO airplane (capacity, type_id, airline_id) VALUES
(186, 2, 1),
(189, 1, 1),
(180, 2, 2),
(100, 3, 2),
(290, 4, 1),
(175, 1, 2);
GO

SELECT * FROM airplane a 
        JOIN airplane_type at ON a.type_id=at.type_id
        JOIN airline ai ON a.airline_id=ai.airline_id
        JOIN airport aa ON ai.base_airport=aa.airport_id

SELECT * FROM airplane
SELECT * FROM airplane_type
SELECT * FROM airline 
SELECT * FROM airport