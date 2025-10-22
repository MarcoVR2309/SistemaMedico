using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Sedes
    {
        private string id;
        private string nomSede;
        private string dirSede;

        public Sedes()
        {
            this.id = "";
            this.nomSede = "";
            this.dirSede = "";
        }

        public Sedes(string id, string nomSede, string dirSede)
        {
            this.id = id;
            this.nomSede = nomSede;
            this.dirSede = dirSede;
        }

        public string Id { get => id; set => id = value; }
        public string NomSede { get => nomSede; set => nomSede = value; }
        public string DirSede { get => dirSede; set => dirSede = value; }
    }
}
