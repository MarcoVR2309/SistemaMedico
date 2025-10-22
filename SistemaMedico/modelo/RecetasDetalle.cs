using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class RecetasDetalle
    {
        private string id;
        private string idRec;
        private string medicamento;
        private string dosis;
        private string frecuencia;
        private string duracion;
        private string viaAdministracion;
        private string indicacionesEspecificas;
        private string cantidad;

        public RecetasDetalle()
        {
            this.id = "";
            this.idRec = "";
            this.medicamento = "";
            this.dosis = "";
            this.frecuencia = "";
            this.duracion = "";
            this.viaAdministracion = "";
            this.indicacionesEspecificas = "";
            this.cantidad = "";
        }

        public RecetasDetalle(string id, string idRec, string medicamento, string dosis, 
                             string frecuencia, string duracion, string viaAdministracion, 
                             string indicacionesEspecificas, string cantidad)
        {
            this.id = id;
            this.idRec = idRec;
            this.medicamento = medicamento;
            this.dosis = dosis;
            this.frecuencia = frecuencia;
            this.duracion = duracion;
            this.viaAdministracion = viaAdministracion;
            this.indicacionesEspecificas = indicacionesEspecificas;
            this.cantidad = cantidad;
        }

        public string Id { get => id; set => id = value; }
        public string IdRec { get => idRec; set => idRec = value; }
        public string Medicamento { get => medicamento; set => medicamento = value; }
        public string Dosis { get => dosis; set => dosis = value; }
        public string Frecuencia { get => frecuencia; set => frecuencia = value; }
        public string Duracion { get => duracion; set => duracion = value; }
        public string ViaAdministracion { get => viaAdministracion; set => viaAdministracion = value; }
        public string IndicacionesEspecificas { get => indicacionesEspecificas; set => indicacionesEspecificas = value; }
        public string Cantidad { get => cantidad; set => cantidad = value; }
    }
}
