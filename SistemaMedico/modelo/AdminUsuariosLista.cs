using System.Collections.Generic;

namespace SistemaMedico.modelo
{
    public class AdminUsuariosLista
    {
        public AdminUsuarioResumen Resumen { get; set; }
        public List<AdminUsuarioItem> Usuarios { get; set; }

        public AdminUsuariosLista()
        {
            Resumen = new AdminUsuarioResumen();
            Usuarios = new List<AdminUsuarioItem>();
        }
    }
}

