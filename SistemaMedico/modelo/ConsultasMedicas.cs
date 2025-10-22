using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class ConsultasMedicas
    {
        private string id;
        private string idCita;
        private string sintomas;
        private string diagnostico;
        private string tratamiento;
        private string observaciones;
        private DateTime fecCon;

        public ConsultasMedicas()
        {
            this.id = "";
            this.idCita = "";
            this.sintomas = "";
            this.diagnostico = "";
            this.tratamiento = "";
            this.observaciones = "";
            this.fecCon = DateTime.Now;
        }

        public ConsultasMedicas(string id, string idCita, string sintomas, string diagnostico, 
                               string tratamiento, string observaciones, DateTime fecCon)
        {
            this.id = id;
            this.idCita = idCita;
            this.sintomas = sintomas;
            this.diagnostico = diagnostico;
            this.tratamiento = tratamiento;
            this.observaciones = observaciones;
            this.fecCon = fecCon;
        }

        public string Id { get => id; set => id = value; }
        public string IdCita { get => idCita; set => idCita = value; }
        public string Sintomas { get => sintomas; set => sintomas = value; }
        public string Diagnostico { get => diagnostico; set => diagnostico = value; }
        public string Tratamiento { get => tratamiento; set => tratamiento = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }
        public DateTime FecCon { get => fecCon; set => fecCon = value; }
    }
}
