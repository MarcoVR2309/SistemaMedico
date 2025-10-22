using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Especialidades
    {
        private string id;
        private string nomEsp;
        private string desEsp;

        public Especialidades()
        {
            this.id = "";
            this.nomEsp = "";
            this.desEsp = "";
        }

        public Especialidades(string id, string nomEsp, string desEsp)
        {
            this.id = id;
            this.nomEsp = nomEsp;
            this.desEsp = desEsp;
        }

        public string Id { get => id; set => id = value; }
        public string NomEsp { get => nomEsp; set => nomEsp = value; }
        public string DesEsp { get => desEsp; set => desEsp = value; }
    }
}
