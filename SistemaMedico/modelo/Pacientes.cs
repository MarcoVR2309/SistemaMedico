using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Pacientes
    {
        private string id;
        private string idUsu;
        private decimal? peso;
        private DateTime? edad;

        public Pacientes()
        {
            this.id = "";
            this.idUsu = "";
            this.peso = 0;
            this.edad = null;
        }

        public Pacientes(string id, string idUsu, decimal? peso, DateTime? edad)
        {
            this.id = id;
            this.idUsu = idUsu;
            this.peso = peso;
            this.edad = edad;
        }

        public string Id { get => id; set => id = value; }
        public string IdUsu { get => idUsu; set => idUsu = value; }
        public decimal? Peso { get => peso; set => peso = value; }
        public DateTime? Edad { get => edad; set => edad = value; }
    }
}
