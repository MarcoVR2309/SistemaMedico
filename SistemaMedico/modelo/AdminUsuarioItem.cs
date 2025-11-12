using System;

namespace SistemaMedico.modelo
{
    public class AdminUsuarioItem
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string IdDoctor { get; set; }
        public string NumeroColegiatura { get; set; }
        public int? AniosExperiencia { get; set; }
        public string IdEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; }
        public string IdPaciente { get; set; }
    }
}

