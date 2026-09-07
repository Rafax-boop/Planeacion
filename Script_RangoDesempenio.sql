-- =====================================================
-- Rangos de Desempeño - configurables desde la BD
-- Base de datos: METAS
-- =====================================================

IF OBJECT_ID('dbo.RangoDesempenio', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RangoDesempenio (
        Id                     INT IDENTITY(1,1) PRIMARY KEY,
        Nombre                 NVARCHAR(50)   NOT NULL,
        Minimo                 DECIMAL(5,2)   NOT NULL,
        Maximo                 DECIMAL(5,2)   NULL,  -- NULL = sin tope superior ("en adelante")
        ClaseColor             NVARCHAR(20)   NOT NULL,
        RequiereJustificacion  BIT            NOT NULL DEFAULT 0,
        Orden                  INT            NOT NULL
    );
END
GO

-- Inserción inicial de los rangos por defecto
IF NOT EXISTS (SELECT 1 FROM dbo.RangoDesempenio)
BEGIN
    INSERT INTO dbo.RangoDesempenio (Nombre, Minimo, Maximo, ClaseColor, RequiereJustificacion, Orden)
    VALUES
        (N'Insuficiente', 0,      70,     N'error',   1, 1),
        (N'Moderado',     70.01,  80,     N'alerta',  0, 2),
        (N'Aceptable',    80.01,  90,     N'warning', 0, 3),
        (N'Óptimo',       90.01,  110,    N'success', 0, 4),
        (N'Excedido',     110.01, NULL,   N'info',    1, 5);
END
GO

SELECT * FROM dbo.RangoDesempenio ORDER BY Orden;
GO