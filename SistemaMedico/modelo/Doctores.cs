using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Doctores
    {
        private string id;
        private string idUsu;
        private string idEsp;
        private string codMed;
        private string desPro;
        private int? expe;

        public Doctores()
        {
            this.id = "";
            this.idUsu = "";
            this.idEsp = "";
            this.codMed = "";
            this.desPro = "";
            this.expe = 0;
        }

        public Doctores(string id, string idUsu, string idEsp, string codMed, string desPro, int? expe)
        {
            this.id = id;
            this.idUsu = idUsu;
            this.idEsp = idEsp;
            this.codMed = codMed;
            this.desPro = desPro;
            this.expe = expe;
        }

        public string Id { get => id; set => id = value; }
        public string IdUsu { get => idUsu; set => idUsu = value; }
        public string IdEsp { get => idEsp; set => idEsp = value; }
        public string CodMed { get => codMed; set => codMed = value; }
        public string DesPro { get => desPro; set => desPro = value; }
        public int? Expe { get => expe; set => expe = value; }

        //Propiedades de navegacion (JOIN)
        public string Nom { get; set; } // Nombre del Usuario
        public string Ape { get; set; } // Apellido del Usuario
        public string Email { get; set; } // Email del Usuario
        public string Tel { get; set; } // Teléfono del Usuario

        public string NombreEspecialidad { get; set; } // Nombre de la Especialidad
    }
}
