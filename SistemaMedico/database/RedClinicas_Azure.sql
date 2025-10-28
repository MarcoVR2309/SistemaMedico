-- ========================================
-- Script de Base de Datos: RedClinicas
-- Versión Azure SQL Database
-- Fecha: 28/10/2025
-- ========================================

-- NOTA: Este script debe ejecutarse en Azure SQL Database
-- NO incluye CREATE DATABASE porque Azure ya lo crea

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ========================================
-- CREAR TABLAS
-- ========================================

-- Tabla: Roles
CREATE TABLE [dbo].[Roles](
	[ID] [varchar](20) NOT NULL,
	[NOM_ROL] [varchar](50) NOT NULL,
	CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [UQ_Roles_NomRol] UNIQUE NONCLUSTERED ([NOM_ROL] ASC)
);
GO

-- Tabla: Sedes
CREATE TABLE [dbo].[Sedes](
	[ID] [varchar](20) NOT NULL,
	[NOM_SEDE] [varchar](100) NOT NULL,
	[DIR_SEDE] [varchar](255) NULL,
	CONSTRAINT [PK_Sedes] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Tabla: Especialidades
CREATE TABLE [dbo].[Especialidades](
	[ID] [varchar](20) NOT NULL,
	[NOM_ESP] [varchar](100) NOT NULL,
	[DES_ESP] [varchar](255) NULL,
	CONSTRAINT [PK_Especialidades] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Tabla: Usuarios
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
	[Activo] [bit] NULL CONSTRAINT [DF_Usuarios_Activo] DEFAULT ((1)),
	[FechaCreacion] [datetime] NULL CONSTRAINT [DF_Usuarios_FechaCreacion] DEFAULT (getdate()),
	[FechaUltimaActualizacion] [datetime] NULL CONSTRAINT [DF_Usuarios_FechaUltAct] DEFAULT (getdate()),
	CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [UQ_Usuarios_Email] UNIQUE NONCLUSTERED ([EMAIL] ASC),
	CONSTRAINT [UQ_Usuarios_NumDoc] UNIQUE NONCLUSTERED ([NUM_DOC] ASC),
	CONSTRAINT [CHK_Usuarios_Gen] CHECK ([GEN]='F' OR [GEN]='M'),
	CONSTRAINT [CHK_Usuarios_TipoDoc] CHECK ([TIPO_DOC]='Pasaporte' OR [TIPO_DOC]='CarnetExtranjeria' OR [TIPO_DOC]='DNI')
);
GO

-- Tabla: Pacientes
CREATE TABLE [dbo].[Pacientes](
	[ID] [varchar](20) NOT NULL,
	[ID_USU] [varchar](20) NOT NULL,
	[PESO] [decimal](10, 2) NULL,
	[EDAD] [date] NULL,
	CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [UQ_Pacientes_IdUsu] UNIQUE NONCLUSTERED ([ID_USU] ASC)
);
GO

-- Tabla: Doctores
CREATE TABLE [dbo].[Doctores](
	[ID] [varchar](20) NOT NULL,
	[ID_USU] [varchar](20) NOT NULL,
	[ID_ESP] [varchar](20) NOT NULL,
	[COD_MED] [varchar](50) NULL,
	[DES_PRO] [text] NULL,
	[EXPE] [int] NULL,
	CONSTRAINT [PK_Doctores] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [UQ_Doctores_CodMed] UNIQUE NONCLUSTERED ([COD_MED] ASC),
	CONSTRAINT [UQ_Doctores_IdUsu] UNIQUE NONCLUSTERED ([ID_USU] ASC)
);
GO

-- Tabla: HorariosDoctor
CREATE TABLE [dbo].[HorariosDoctor](
	[ID] [varchar](20) NOT NULL,
	[ID_DOC] [varchar](20) NOT NULL,
	[ID_SEDE] [varchar](20) NOT NULL,
	[DIA_SEM] [int] NOT NULL,
	[HORA] [time](7) NOT NULL,
	[HORA_FIN] [time](7) NOT NULL,
	CONSTRAINT [PK_HorariosDoctor] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [CHK_HorariosDoctor_DiaSem] CHECK ([DIA_SEM]>=(1) AND [DIA_SEM]<=(7))
);
GO

-- Tabla: Citas
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
	[PAGO_REALI] [bit] NULL CONSTRAINT [DF_Citas_PagoReali] DEFAULT ((0)),
	[MOTIVO] [text] NULL,
	CONSTRAINT [PK_Citas] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [CHK_Citas_TipoPago] CHECK ([TIPO_PAGO]='TarjetaOnline' OR [TIPO_PAGO]='Presencial')
);
GO

-- Tabla: ConsultasMedicas
CREATE TABLE [dbo].[ConsultasMedicas](
	[ID] [varchar](20) NOT NULL,
	[ID_CITA] [varchar](20) NOT NULL,
	[SINTOMAS] [text] NULL,
	[DIAGNOSTICO] [text] NULL,
	[TRATAMIENTO] [text] NULL,
	[OBSERVACIONES] [text] NULL,
	[FEC_CON] [datetime] NULL CONSTRAINT [DF_ConsultasMedicas_FecCon] DEFAULT (getdate()),
	CONSTRAINT [PK_ConsultasMedicas] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Tabla: RecetasMedicas
CREATE TABLE [dbo].[RecetasMedicas](
	[ID] [varchar](20) NOT NULL,
	[ID_CON] [varchar](20) NOT NULL,
	[FECHA_RECETA] [datetime] NULL CONSTRAINT [DF_RecetasMedicas_FechaReceta] DEFAULT (getdate()),
	CONSTRAINT [PK_RecetasMedicas] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Tabla: RecetasDetalle
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
	CONSTRAINT [PK_RecetasDetalle] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Tabla: Pagos
CREATE TABLE [dbo].[Pagos](
	[ID] [varchar](20) NOT NULL,
	[ID_CITA] [varchar](20) NOT NULL,
	[MONTO] [decimal](10, 2) NOT NULL,
	[MET_PAGO] [varchar](20) NULL,
	[EST_PAGO] [varchar](20) NULL,
	[ID_TRANS] [varchar](100) NULL,
	[FEC] [datetime] NULL,
	CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [CHK_Pagos_EstPago] CHECK ([EST_PAGO]='Reembolsado' OR [EST_PAGO]='Fallido' OR [EST_PAGO]='Completado' OR [EST_PAGO]='Pendiente'),
	CONSTRAINT [CHK_Pagos_MetPago] CHECK ([MET_PAGO]='TarjetaOnline' OR [MET_PAGO]='Presencial')
);
GO

-- ========================================
-- FOREIGN KEYS
-- ========================================

-- Usuarios
ALTER TABLE [dbo].[Usuarios] WITH CHECK ADD 
	CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY([ID_ROL])
	REFERENCES [dbo].[Roles] ([ID]);
GO

ALTER TABLE [dbo].[Usuarios] WITH CHECK ADD 
	CONSTRAINT [FK_Usuarios_Sedes] FOREIGN KEY([SEDE_PREF])
	REFERENCES [dbo].[Sedes] ([ID]);
GO

-- Pacientes
ALTER TABLE [dbo].[Pacientes] WITH CHECK ADD 
	CONSTRAINT [FK_Pacientes_Usuarios] FOREIGN KEY([ID_USU])
	REFERENCES [dbo].[Usuarios] ([ID]);
GO

-- Doctores
ALTER TABLE [dbo].[Doctores] WITH CHECK ADD 
	CONSTRAINT [FK_Doctores_Usuarios] FOREIGN KEY([ID_USU])
	REFERENCES [dbo].[Usuarios] ([ID]);
GO

ALTER TABLE [dbo].[Doctores] WITH CHECK ADD 
	CONSTRAINT [FK_Doctores_Especialidades] FOREIGN KEY([ID_ESP])
	REFERENCES [dbo].[Especialidades] ([ID]);
GO

-- HorariosDoctor
ALTER TABLE [dbo].[HorariosDoctor] WITH CHECK ADD 
	CONSTRAINT [FK_HorariosDoctor_Doctores] FOREIGN KEY([ID_DOC])
	REFERENCES [dbo].[Doctores] ([ID]);
GO

ALTER TABLE [dbo].[HorariosDoctor] WITH CHECK ADD 
	CONSTRAINT [FK_HorariosDoctor_Sedes] FOREIGN KEY([ID_SEDE])
	REFERENCES [dbo].[Sedes] ([ID]);
GO

-- Citas
ALTER TABLE [dbo].[Citas] WITH CHECK ADD 
	CONSTRAINT [FK_Citas_Pacientes] FOREIGN KEY([ID_PAC])
	REFERENCES [dbo].[Pacientes] ([ID]);
GO

ALTER TABLE [dbo].[Citas] WITH CHECK ADD 
	CONSTRAINT [FK_Citas_Doctores] FOREIGN KEY([ID_DOC])
	REFERENCES [dbo].[Doctores] ([ID]);
GO

ALTER TABLE [dbo].[Citas] WITH CHECK ADD 
	CONSTRAINT [FK_Citas_Sedes] FOREIGN KEY([ID_SEDE])
	REFERENCES [dbo].[Sedes] ([ID]);
GO

ALTER TABLE [dbo].[Citas] WITH CHECK ADD 
	CONSTRAINT [FK_Citas_Especialidades] FOREIGN KEY([ID_ESP])
	REFERENCES [dbo].[Especialidades] ([ID]);
GO

-- ConsultasMedicas
ALTER TABLE [dbo].[ConsultasMedicas] WITH CHECK ADD 
	CONSTRAINT [FK_ConsultasMedicas_Citas] FOREIGN KEY([ID_CITA])
	REFERENCES [dbo].[Citas] ([ID]);
GO

-- RecetasMedicas
ALTER TABLE [dbo].[RecetasMedicas] WITH CHECK ADD 
	CONSTRAINT [FK_RecetasMedicas_ConsultasMedicas] FOREIGN KEY([ID_CON])
	REFERENCES [dbo].[ConsultasMedicas] ([ID]);
GO

-- RecetasDetalle
ALTER TABLE [dbo].[RecetasDetalle] WITH CHECK ADD 
	CONSTRAINT [FK_RecetasDetalle_RecetasMedicas] FOREIGN KEY([ID_REC])
	REFERENCES [dbo].[RecetasMedicas] ([ID]);
GO

-- Pagos
ALTER TABLE [dbo].[Pagos] WITH CHECK ADD 
	CONSTRAINT [FK_Pagos_Citas] FOREIGN KEY([ID_CITA])
	REFERENCES [dbo].[Citas] ([ID]);
GO

-- ========================================
-- DATOS INICIALES (OPCIONAL)
-- ========================================

-- Insertar Roles básicos
INSERT INTO [dbo].[Roles] ([ID], [NOM_ROL]) VALUES 
('ROL001', 'Administrador'),
('ROL002', 'Doctor'),
('ROL003', 'Paciente'),
('ROL004', 'Recepcionista');
GO

-- Insertar Sedes ejemplo
INSERT INTO [dbo].[Sedes] ([ID], [NOM_SEDE], [DIR_SEDE]) VALUES 
('SEDE001', 'Sede Principal - Lima Centro', 'Av. Arequipa 1234, Miraflores'),
('SEDE002', 'Sede Norte - Los Olivos', 'Av. Alfredo Mendiola 5678, Los Olivos'),
('SEDE003', 'Sede Sur - Surco', 'Av. Benavides 9012, Santiago de Surco');
GO

-- Insertar Especialidades
INSERT INTO [dbo].[Especialidades] ([ID], [NOM_ESP], [DES_ESP]) VALUES 
('ESP001', 'Medicina General', 'Atención médica general y preventiva'),
('ESP002', 'Cardiología', 'Especialidad del corazón y sistema cardiovascular'),
('ESP003', 'Pediatría', 'Atención médica para niños y adolescentes'),
('ESP004', 'Ginecología', 'Salud de la mujer y sistema reproductivo'),
('ESP005', 'Traumatología', 'Tratamiento de lesiones del sistema musculoesquelético'),
('ESP006', 'Dermatología', 'Tratamiento de enfermedades de la piel'),
('ESP007', 'Oftalmología', 'Cuidado de la salud visual'),
('ESP008', 'Neurología', 'Enfermedades del sistema nervioso');
GO

PRINT 'Base de datos RedClinicas creada exitosamente en Azure SQL Database';
PRINT 'Total de tablas: 12';
PRINT 'Foreign Keys: Configuradas correctamente';
PRINT 'Datos iniciales: Roles, Sedes y Especialidades insertados';
GO
