PRAGMA foreign_keys = ON;

-- Propietarios
CREATE TABLE IF NOT EXISTS Propietarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DNI TEXT NOT NULL,
    Apellido TEXT NOT NULL,
    Nombre TEXT NOT NULL,
    Telefono TEXT NULL,
    Email TEXT NULL
);

-- Inquilinos
CREATE TABLE IF NOT EXISTS Inquilinos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DNI TEXT NOT NULL,
    NombreCompleto TEXT NOT NULL,
    Telefono TEXT NULL,
    Email TEXT NULL
);

-- Tipos de Inmueble (ABM)
CREATE TABLE IF NOT EXISTS TiposInmuebles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL UNIQUE,
    Descripcion TEXT NULL
);

-- Inmuebles (mantengo Tipo TEXT para compatibilidad con tu código actual)
CREATE TABLE IF NOT EXISTS Inmuebles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Direccion TEXT NOT NULL,
    Uso TEXT NOT NULL,
    Tipo TEXT NOT NULL,
    Ambientes INTEGER NOT NULL,
    Superficie REAL NULL,
    Latitud REAL NULL,
    Longitud REAL NULL,
    PrecioBase REAL NOT NULL,
    IdPropietario INTEGER NOT NULL,
    Estado TEXT NOT NULL DEFAULT 'Disponible',
    FOREIGN KEY (IdPropietario) REFERENCES Propietarios(Id) ON DELETE RESTRICT
);

-- Contratos con auditoría
CREATE TABLE IF NOT EXISTS Contratos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IdInmueble INTEGER NOT NULL,
    IdInquilino INTEGER NOT NULL,
    FechaInicio TEXT NOT NULL,
    FechaFin TEXT NOT NULL,
    MontoMensual REAL NOT NULL,
    Deposito REAL NULL,
    Estado TEXT NOT NULL DEFAULT 'Vigente',
    CreatedByUserId INTEGER NULL,
    CreatedAt TEXT NULL,
    EndedByUserId INTEGER NULL,
    EndedAt TEXT NULL,
    FOREIGN KEY (IdInmueble) REFERENCES Inmuebles(Id) ON DELETE RESTRICT,
    FOREIGN KEY (IdInquilino) REFERENCES Inquilinos(Id) ON DELETE RESTRICT
);

-- Pagos con auditoría y estado (Anulado)
CREATE TABLE IF NOT EXISTS Pagos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IdContrato INTEGER NOT NULL,
    NroPago INTEGER NOT NULL,
    Fecha TEXT NOT NULL,
    Importe REAL NOT NULL,
    Detalle TEXT NULL,
    Estado TEXT NOT NULL DEFAULT 'Activo', -- Activo / Anulado
    CreatedByUserId INTEGER NULL,
    CreatedAt TEXT NULL,
    AnnulledByUserId INTEGER NULL,
    AnnulledAt TEXT NULL,
    FOREIGN KEY (IdContrato) REFERENCES Contratos(Id) ON DELETE CASCADE
);

-- Usuarios (auth)
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Email TEXT NOT NULL UNIQUE,
    Nombre TEXT NOT NULL,
    Apellido TEXT NULL,
    PasswordHash TEXT NOT NULL,
    Rol TEXT NOT NULL, -- 'Admin' o 'Empleado'
    AvatarPath TEXT NULL,
    CreatedAt TEXT NULL
);

-- Índices útiles
CREATE INDEX IF NOT EXISTS IX_Inmuebles_IdPropietario ON Inmuebles(IdPropietario);
CREATE INDEX IF NOT EXISTS IX_Contratos_IdInmueble ON Contratos(IdInmueble);
CREATE INDEX IF NOT EXISTS IX_Contratos_IdInquilino ON Contratos(IdInquilino);
CREATE INDEX IF NOT EXISTS IX_Pagos_IdContrato ON Pagos(IdContrato);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Pagos_ContratoNro ON Pagos(IdContrato, NroPago);
