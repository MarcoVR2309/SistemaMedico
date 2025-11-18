using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Citas
    {
        // --- Campos privados (Columnas de la BD) ---
        private string id;
        private string idPac;
        private string idDoc;
        private string idSede;
        private string idEsp;
        private DateTime fecha;
        private TimeSpan hora;
        private string estado;
        private string tipoPago;
        private decimal? monto;
        private bool pagoReali;
        private string motivo;

        // --- Constructores ---
        public Citas()
        {
            this.id = "";
            this.idPac = "";
            this.idDoc = "";
            this.idSede = "";
            this.idEsp = "";
            this.fecha = DateTime.Now;
            this.hora = TimeSpan.Zero;
            this.estado = "Pendiente";
            this.tipoPago = "";
            this.monto = 0;
            this.pagoReali = false;
            this.motivo = "";
        }

        public Citas(string id, string idPac, string idDoc, string idSede, string idEsp,
                    DateTime fecha, TimeSpan hora, string estado, string tipoPago,
                    decimal? monto, bool pagoReali, string motivo)
        {
            this.id = id;
            this.idPac = idPac;
            this.idDoc = idDoc;
            this.idSede = idSede;
            this.idEsp = idEsp;
            this.fecha = fecha;
            this.hora = hora;
            this.estado = estado;
            this.tipoPago = tipoPago;
            this.monto = monto;
            this.pagoReali = pagoReali;
            this.motivo = motivo;
        }
        // Propiedades calculadas usadas por el Repeater del PanelPaciente
        public string FechaCita
        {
            get { return Fecha.ToString("dd/MM/yyyy"); }
        }

        public string HoraCita
        {
            get { return Hora.ToString(@"hh\:mm"); }
        }


        // --- Propiedades públicas (Get/Set) ---
        public string Id { get => id; set => id = value; }
        public string IdPac { get => idPac; set => idPac = value; }
        public string IdDoc { get => idDoc; set => idDoc = value; }
        public string IdSede { get => idSede; set => idSede = value; }
        public string IdEsp { get => idEsp; set => idEsp = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }
        public TimeSpan Hora { get => hora; set => hora = value; }
        public string Estado { get => estado; set => estado = value; }
        public string TipoPago { get => tipoPago; set => tipoPago = value; }
        public decimal? Monto { get => monto; set => monto = value; }
        public bool PagoReali { get => pagoReali; set => pagoReali = value; }
        public string Motivo { get => motivo; set => motivo = value; }

        // =========================================================================
        // PROPIEDADES DE NAVEGACIÓN (Para mostrar en GridView/Repeater)
        // Estas NO se guardan en la BD, solo se usan para mostrar información
        // que viene de un JOIN con otras tablas
        // =========================================================================

        /// <summary>
        /// Nombre completo del doctor (viene del JOIN con tabla Doctores y Usuarios)
        /// </summary>
        public string NombreDoctor { get; set; }

        /// <summary>
        /// Nombre de la especialidad (viene del JOIN con tabla Especialidades)
        /// </summary>
        public string NombreEspecialidad { get; set; }

        /// <summary>
        /// Nombre de la sede (viene del JOIN con tabla Sedes)
        /// </summary>
        public string SedeNombre { get; set; }

        // Propiedades adicionales opcionales que puedes necesitar en el futuro:

        /// <summary>
        /// Nombre completo del paciente (opcional, para panel del doctor)
        /// </summary>
        public string NombrePaciente { get; set; }

        /// <summary>
        /// DNI del paciente (opcional)
        /// </summary>
        public string DniPaciente { get; set; }
    }

}