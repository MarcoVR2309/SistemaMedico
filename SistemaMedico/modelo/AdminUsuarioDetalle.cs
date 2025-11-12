using System;

namespace SistemaMedico.modelo
{
    public class AdminUsuarioDetalle
    {
        public string Id { get; set; }
        public string IdRol { get; set; }
        public string NombreRol { get; set; }
        public string TipoDoc { get; set; }
        public string NumDoc { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public char Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string SedePreferida { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public string IdDoctor { get; set; }
        public string IdEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; }
        public string NumeroColegiatura { get; set; }
        public int? AniosExperiencia { get; set; }
        public string IdPaciente { get; set; }
        public decimal? PesoPaciente { get; set; }
        public DateTime? EdadPaciente { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}

