PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Propietarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DNI TEXT NOT NULL,
    Apellido TEXT NOT NULL,
    Nombre TEXT NOT NULL,
    Telefono TEXT NULL,
    Email TEXT NULL
);

CREATE TABLE IF NOT EXISTS Inquilinos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DNI TEXT NOT NULL,
    NombreCompleto TEXT NOT NULL,
    Telefono TEXT NULL,
    Email TEXT NULL
);

CREATE TABLE IF NOT EXISTS Inmuebles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Direccion TEXT NOT NULL,
    Uso TEXT NOT NULL,            -- Ej: "Residencial", "Comercial"
    Tipo TEXT NOT NULL,           -- Ej: "Casa", "Departamento", "Local"
    Ambientes INTEGER NOT NULL,
    Superficie REAL NULL,
    Latitud REAL NULL,
    Longitud REAL NULL,
    PrecioBase REAL NOT NULL,     -- Precio de referencia sugerido
    IdPropietario INTEGER NOT NULL,
    FOREIGN KEY (IdPropietario) REFERENCES Propietarios(Id) ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS Contratos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IdInmueble INTEGER NOT NULL,
    IdInquilino INTEGER NOT NULL,
    FechaInicio TEXT NOT NULL,    -- ISO yyyy-MM-dd
    FechaFin TEXT NOT NULL,       -- ISO yyyy-MM-dd
    MontoMensual REAL NOT NULL,
    Deposito REAL NULL,
    Estado TEXT NOT NULL DEFAULT 'Vigente', -- 'Vigente','Finalizado','Rescindido'
    FOREIGN KEY (IdInmueble) REFERENCES Inmuebles(Id) ON DELETE RESTRICT,
    FOREIGN KEY (IdInquilino) REFERENCES Inquilinos(Id) ON DELETE RESTRICT
);

-- Opcional: para avanzar con pagos
CREATE TABLE IF NOT EXISTS Pagos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IdContrato INTEGER NOT NULL,
    NroPago INTEGER NOT NULL,
    Fecha TEXT NOT NULL,          -- ISO yyyy-MM-dd
    Importe REAL NOT NULL,
    FOREIGN KEY (IdContrato) REFERENCES Contratos(Id) ON DELETE CASCADE
);

-- Índices útiles
CREATE INDEX IF NOT EXISTS IX_Inmuebles_IdPropietario ON Inmuebles(IdPropietario);
CREATE INDEX IF NOT EXISTS IX_Contratos_IdInmueble ON Contratos(IdInmueble);
CREATE INDEX IF NOT EXISTS IX_Contratos_IdInquilino ON Contratos(IdInquilino);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Pagos_ContratoNro ON Pagos(IdContrato, NroPago);