using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class RecetasMedicas
    {
        private string id;
        private string idCon;
        private DateTime fechaReceta;

        public RecetasMedicas()
        {
            this.id = "";
            this.idCon = "";
            this.fechaReceta = DateTime.Now;
        }

        public RecetasMedicas(string id, string idCon, DateTime fechaReceta)
        {
            this.id = id;
            this.idCon = idCon;
            this.fechaReceta = fechaReceta;
        }

        public string Id { get => id; set => id = value; }
        public string IdCon { get => idCon; set => idCon = value; }
        public DateTime FechaReceta { get => fechaReceta; set => fechaReceta = value; }
    }
}
