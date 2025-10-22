using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Rol
    {
        private string id;
        private string nomRol;

        public Rol()
        {
            this.id = "";
            this.nomRol = "";
        }

        public Rol(string id, string nomRol)
        {
            this.id = id;
            this.nomRol = nomRol;
        }

        public string Id { get => id; set => id = value; }
        public string NomRol { get => nomRol; set => nomRol = value; }
    }
}
