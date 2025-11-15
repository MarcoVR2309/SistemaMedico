-- =========================================================================
-- SCRIPT DE CREACIÓN DE TABLAS - RED CLÍNICAS
-- Base de datos: RedCLinicas (Azure SQL Database)
-- Fecha: 15/11/2025
-- =========================================================================

USE RedCLinicas;
GO

PRINT 'Iniciando creación de tablas...';
GO

-- =========================================================================
-- TABLA: ROLES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles](
        [ID] [varchar](20) NOT NULL,
        [NOM_ROL] [varchar](50) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Roles_NomRol] UNIQUE NONCLUSTERED ([NOM_ROL] ASC)
    );
    PRINT 'Tabla Roles creada.';
END
GO

-- =========================================================================
-- TABLA: SEDES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sedes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Sedes](
        [ID] [varchar](20) NOT NULL,
        [NOM_SEDE] [varchar](100) NOT NULL,
        [DIR_SEDE] [varchar](255) NULL,
        CONSTRAINT [PK_Sedes] PRIMARY KEY CLUSTERED ([ID] ASC)
    );
    PRINT 'Tabla Sedes creada.';
END
GO

-- =========================================================================
-- TABLA: ESPECIALIDADES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Especialidades]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Especialidades](
        [ID] [varchar](20) NOT NULL,
        [NOM_ESP] [varchar](100) NOT NULL,
        [DES_ESP] [varchar](255) NULL,
        CONSTRAINT [PK_Especialidades] PRIMARY KEY CLUSTERED ([ID] ASC)
    );
    PRINT 'Tabla Especialidades creada.';
END
GO

-- =========================================================================
-- TABLA: USUARIOS
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuarios](
        [ID] [varchar](20) NOT NULL,
        [ID_ROL] [varchar](20) NOT NULL,
        [TIPO_DOC] [varchar](20) NOT NULL,
        [NUM_DOC] [varchar](20) NOT NULL,
        [APE] [varchar](100) NOT NULL,
        [NOM] [varchar](100) NOT NULL,
        [GEN] [char](1) NULL,
        [FEC_NAC] [date] NULL,
        [EMAIL] [varchar](100) NOT NULL,
        [TEL] [varchar](20) NULL,
        [PSWDHASH] [varbinary](256) NOT NULL,
        [SEDE_PREF] [varchar](20) NULL,
        [Activo] [bit] NULL CONSTRAINT [DF_Usuarios_Activo] DEFAULT (1),
        [FechaCreacion] [datetime] NULL CONSTRAINT [DF_Usuarios_FechaCreacion] DEFAULT (getdate()),
        [FechaUltimaActualizacion] [datetime] NULL CONSTRAINT [DF_Usuarios_FechaUltAct] DEFAULT (getdate()),
        
        CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Usuarios_Email] UNIQUE NONCLUSTERED ([EMAIL] ASC),
        CONSTRAINT [UQ_Usuarios_NumDoc] UNIQUE NONCLUSTERED ([NUM_DOC] ASC),
        CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY([ID_ROL]) REFERENCES [dbo].[Roles] ([ID]),
        CONSTRAINT [FK_Usuarios_Sedes] FOREIGN KEY([SEDE_PREF]) REFERENCES [dbo].[Sedes] ([ID]),
        CONSTRAINT [CHK_Usuarios_Gen] CHECK ([GEN]='F' OR [GEN]='M'),
        CONSTRAINT [CHK_Usuarios_TipoDoc] CHECK ([TIPO_DOC]='Pasaporte' OR [TIPO_DOC]='CarnetExtranjeria' OR [TIPO_DOC]='DNI')
    );
    PRINT 'Tabla Usuarios creada.';
END
GO

-- =========================================================================
-- TABLA: PACIENTES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pacientes](
        [ID] [varchar](20) NOT NULL,
        [ID_USU] [varchar](20) NOT NULL,
        [PESO] [decimal](10, 2) NULL,
        [EDAD] [date] NULL,
        
        CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Pacientes_IdUsu] UNIQUE NONCLUSTERED ([ID_USU] ASC),
        CONSTRAINT [FK_Pacientes_Usuarios] FOREIGN KEY([ID_USU]) REFERENCES [dbo].[Usuarios] ([ID])
    );
    PRINT 'Tabla Pacientes creada.';
END
GO

-- =========================================================================
-- TABLA: DOCTORES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Doctores]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Doctores](
        [ID] [varchar](20) NOT NULL,
        [ID_USU] [varchar](20) NOT NULL,
        [ID_ESP] [varchar](20) NOT NULL,
        [COD_MED] [varchar](50) NULL,
        [DES_PRO] [text] NULL,
        [EXPE] [int] NULL,
        
        CONSTRAINT [PK_Doctores] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Doctores_CodMed] UNIQUE NONCLUSTERED ([COD_MED] ASC),
        CONSTRAINT [UQ_Doctores_IdUsu] UNIQUE NONCLUSTERED ([ID_USU] ASC),
        CONSTRAINT [FK_Doctores_Usuarios] FOREIGN KEY([ID_USU]) REFERENCES [dbo].[Usuarios] ([ID]),
        CONSTRAINT [FK_Doctores_Especialidades] FOREIGN KEY([ID_ESP]) REFERENCES [dbo].[Especialidades] ([ID])
    );
    PRINT 'Tabla Doctores creada.';
END
GO

-- =========================================================================
-- TABLA: ADMINISTRADORES
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Administradores]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Administradores](
        [ID] [varchar](20) NOT NULL,
        [ID_USU] [varchar](20) NOT NULL,
        [NIVEL_ACCESO] [int] NOT NULL DEFAULT (1),
        [PERMISOS_ESPECIALES] [varchar](max) NULL,
        [ULTIMA_ACCION] [datetime] NULL,
        [ACCIONES_REALIZADAS] [int] NULL DEFAULT (0),
        
        CONSTRAINT [PK_Administradores] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Administradores_IdUsu] UNIQUE NONCLUSTERED ([ID_USU] ASC),
        CONSTRAINT [FK_Administradores_Usuarios] FOREIGN KEY([ID_USU]) REFERENCES [dbo].[Usuarios] ([ID]),
        CONSTRAINT [CHK_Administradores_NivelAcceso] CHECK ([NIVEL_ACCESO]>=(1) AND [NIVEL_ACCESO]<=(3))
    );
    PRINT 'Tabla Administradores creada.';
END
GO

-- =========================================================================
-- TABLA: HORARIOS DOCTOR
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HorariosDoctor]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[HorariosDoctor](
        [ID] [varchar](20) NOT NULL,
        [ID_DOC] [varchar](20) NOT NULL,
        [ID_SEDE] [varchar](20) NOT NULL,
        [DIA_SEM] [int] NOT NULL,
        [HORA] [time](7) NOT NULL,
        [HORA_FIN] [time](7) NOT NULL,
        
        CONSTRAINT [PK_HorariosDoctor] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_HorariosDoctor_Doctores] FOREIGN KEY([ID_DOC]) REFERENCES [dbo].[Doctores] ([ID]) ON UPDATE CASCADE ON DELETE CASCADE,
        CONSTRAINT [FK_HorariosDoctor_Sedes] FOREIGN KEY([ID_SEDE]) REFERENCES [dbo].[Sedes] ([ID]),
        CONSTRAINT [CHK_HorariosDoctor_DiaSem] CHECK ([DIA_SEM]>=(1) AND [DIA_SEM]<=(7))
    );
    PRINT 'Tabla HorariosDoctor creada.';
END
GO

-- =========================================================================
-- TABLA: CITAS
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Citas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Citas](
        [ID] [varchar](20) NOT NULL,
        [ID_PAC] [varchar](20) NOT NULL,
        [ID_DOC] [varchar](20) NOT NULL,
        [ID_SEDE] [varchar](20) NOT NULL,
        [ID_ESP] [varchar](20) NOT NULL,
        [FECHA] [date] NOT NULL,
        [HORA] [time](7) NOT NULL,
        [ESTADO] [varchar](1) NULL CONSTRAINT [DF_Citas_Estado] DEFAULT ('Pendiente'),
        [TIPO_PAGO] [varchar](20) NULL,
        [MONTO] [decimal](10, 2) NULL,
        [PAGO_REALI] [bit] NULL CONSTRAINT [DF_Citas_PagoReali] DEFAULT (0),
        [MOTIVO] [text] NULL,
        
        CONSTRAINT [PK_Citas] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_Citas_Pacientes] FOREIGN KEY([ID_PAC]) REFERENCES [dbo].[Pacientes] ([ID]),
        CONSTRAINT [FK_Citas_Doctores] FOREIGN KEY([ID_DOC]) REFERENCES [dbo].[Doctores] ([ID]),
        CONSTRAINT [FK_Citas_Sedes] FOREIGN KEY([ID_SEDE]) REFERENCES [dbo].[Sedes] ([ID]),
        CONSTRAINT [FK_Citas_Especialidades] FOREIGN KEY([ID_ESP]) REFERENCES [dbo].[Especialidades] ([ID]),
        CONSTRAINT [CHK_Citas_TipoPago] CHECK ([TIPO_PAGO]='TarjetaOnline' OR [TIPO_PAGO]='Presencial')
    );
    PRINT 'Tabla Citas creada.';
END
GO

-- =========================================================================
-- TABLA: CONSULTAS MEDICAS
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsultasMedicas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsultasMedicas](
        [ID] [varchar](20) NOT NULL,
        [ID_CITA] [varchar](20) NOT NULL,
        [SINTOMAS] [text] NULL,
        [DIAGNOSTICO] [text] NULL,
        [TRATAMIENTO] [text] NULL,
        [OBSERVACIONES] [text] NULL,
        [FEC_CON] [datetime] NULL CONSTRAINT [DF_ConsultasMedicas_FecCon] DEFAULT (getdate()),
        
        CONSTRAINT [PK_ConsultasMedicas] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_ConsultasMedicas_Citas] FOREIGN KEY([ID_CITA]) REFERENCES [dbo].[Citas] ([ID])
    );
    PRINT 'Tabla ConsultasMedicas creada.';
END
GO

-- =========================================================================
-- TABLA: RECETAS MEDICAS
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecetasMedicas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RecetasMedicas](
        [ID] [varchar](20) NOT NULL,
        [ID_CON] [varchar](20) NOT NULL,
        [FECHA_RECETA] [datetime] NULL CONSTRAINT [DF_RecetasMedicas_FechaReceta] DEFAULT (getdate()),
        
        CONSTRAINT [PK_RecetasMedicas] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_RecetasMedicas_ConsultasMedicas] FOREIGN KEY([ID_CON]) REFERENCES [dbo].[ConsultasMedicas] ([ID])
    );
    PRINT 'Tabla RecetasMedicas creada.';
END
GO

-- =========================================================================
-- TABLA: RECETAS DETALLE
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecetasDetalle]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RecetasDetalle](
        [ID] [varchar](20) NOT NULL,
        [ID_REC] [varchar](20) NOT NULL,
        [MEDICAMENTO] [varchar](100) NOT NULL,
        [DOSIS] [varchar](100) NULL,
        [FRECUENCIA] [varchar](50) NULL,
        [DURACION] [varchar](50) NULL,
        [VIA_ADMINISTRACION] [varchar](50) NULL,
        [INDICACIONES_ESPECIFICAS] [text] NULL,
        [CANTIDAD] [varchar](50) NULL,
        
        CONSTRAINT [PK_RecetasDetalle] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_RecetasDetalle_RecetasMedicas] FOREIGN KEY([ID_REC]) REFERENCES [dbo].[RecetasMedicas] ([ID])
    );
    PRINT 'Tabla RecetasDetalle creada.';
END
GO

-- =========================================================================
-- TABLA: PAGOS
-- =========================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pagos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pagos](
        [ID] [varchar](20) NOT NULL,
        [ID_CITA] [varchar](20) NOT NULL,
        [MONTO] [decimal](10, 2) NOT NULL,
        [MET_PAGO] [varchar](20) NULL,
        [EST_PAGO] [varchar](20) NULL,
        [ID_TRANS] [varchar](100) NULL,
        [FEC] [datetime] NULL,
        
        CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_Pagos_Citas] FOREIGN KEY([ID_CITA]) REFERENCES [dbo].[Citas] ([ID]),
        CONSTRAINT [CHK_Pagos_MetPago] CHECK ([MET_PAGO]='TarjetaOnline' OR [MET_PAGO]='Presencial'),
        CONSTRAINT [CHK_Pagos_EstPago] CHECK ([EST_PAGO]='Reembolsado' OR [EST_PAGO]='Fallido' OR [EST_PAGO]='Completado' OR [EST_PAGO]='Pendiente')
    );
    PRINT 'Tabla Pagos creada.';
END
GO

PRINT '=== CREACIÓN DE TABLAS COMPLETADA ===';
GO
