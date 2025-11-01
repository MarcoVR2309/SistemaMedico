using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Administradores
    {
        private string id;
        private string idUsu;
        private int nivelAcceso;
        private string permisosEspeciales;
        private DateTime? ultimaAccion;
        private int accionesRealizadas;

        public Administradores()
        {
            this.id = "";
            this.idUsu = "";
            this.nivelAcceso = 1; // Admin normal por defecto
            this.permisosEspeciales = null;
            this.ultimaAccion = null;
            this.accionesRealizadas = 0;
        }

        public Administradores(string id, string idUsu, int nivelAcceso, string permisosEspeciales, DateTime? ultimaAccion, int accionesRealizadas)
        {
            this.id = id;
            this.idUsu = idUsu;
            this.nivelAcceso = nivelAcceso;
            this.permisosEspeciales = permisosEspeciales;
            this.ultimaAccion = ultimaAccion;
            this.accionesRealizadas = accionesRealizadas;
        }

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string IdUsu
        {
            get => idUsu;
            set => idUsu = value;
        }

        public int NivelAcceso
        {
            get => nivelAcceso;
            set => nivelAcceso = value;
        }

        public string PermisosEspeciales
        {
            get => permisosEspeciales;
            set => permisosEspeciales = value;
        }

        public DateTime? UltimaAccion
        {
            get => ultimaAccion;
            set => ultimaAccion = value;
        }

        public int AccionesRealizadas
        {
            get => accionesRealizadas;
            set => accionesRealizadas = value;
        }
    }
}
