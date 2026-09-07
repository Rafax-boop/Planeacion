-- =====================================================
-- Limpieza de tablas sin uso en la aplicación
-- Base de datos: METAS
-- Tablas vacías, sin FKs entrantes ni referencias en código.
-- sysdiagrams NO se toca (artefacto de SQL Server/SSMS).
-- =====================================================

-- Elimina las FKs salientes (FK_Vinculacion_LlenadoInterno y
-- FK_Vinculacion_Municipio) junto con la tabla.
IF OBJECT_ID('dbo.Vinculacion', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Vinculacion;
END
GO

IF OBJECT_ID('dbo.AnoHabilitar', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.AnoHabilitar;
END
GO

SELECT t.name AS Tabla
FROM sys.tables t
ORDER BY t.name;
GO