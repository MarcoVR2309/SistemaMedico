using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaMedico.vista
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegistro_Click(object sender, EventArgs e)
        {
            // Redirigir a la página de registro
            Response.Redirect("Registro.aspx");
        }

        protected void btnAcceder_Click(object sender, EventArgs e)
        {
            // Redirigir a la página de login de pacientes
            Response.Redirect("LoginPaciente.aspx");
        }

        protected void btnPortalMedico_Click(object sender, EventArgs e)
        {
            // Redirigir al portal médico
            Response.Redirect("LoginMedico.aspx");
        }

        protected void btnVerServicios_Click(object sender, EventArgs e)
        {
            // Redirigir a la página de servicios
            Response.Redirect("Servicios.aspx");
        }

        protected void btnVerMedicos_Click(object sender, EventArgs e)
        {
            // Redirigir a la página de staff médico
            Response.Redirect("StaffMedico.aspx");
        }
    }
}
