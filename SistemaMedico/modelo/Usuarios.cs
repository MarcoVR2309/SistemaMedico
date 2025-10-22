using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Usuarios
    {
        private string id;
        private string idRol;
        private string tipoDoc;
        private string numDoc;
        private string ape;
        private string nom;
        private string gen;
        private DateTime? fecNac;
        private string email;
        private string tel;
        private byte[] pswdHash;
        private string sedePref;
        private bool activo;
        private DateTime fechaCreacion;
        private DateTime fechaUltimaActualizacion;

        public Usuarios()
        {
            this.id = "";
            this.idRol = "";
            this.tipoDoc = "";
            this.numDoc = "";
            this.ape = "";
            this.nom = "";
            this.gen = "";
            this.fecNac = null;
            this.email = "";
            this.tel = "";
            this.pswdHash = null;
            this.sedePref = "";
            this.activo = true;
            this.fechaCreacion = DateTime.Now;
            this.fechaUltimaActualizacion = DateTime.Now;
        }

        public Usuarios(string id, string idRol, string tipoDoc, string numDoc, string ape, string nom, 
                       string gen, DateTime? fecNac, string email, string tel, byte[] pswdHash, 
                       string sedePref, bool activo, DateTime fechaCreacion, DateTime fechaUltimaActualizacion)
        {
            this.id = id;
            this.idRol = idRol;
            this.tipoDoc = tipoDoc;
            this.numDoc = numDoc;
            this.ape = ape;
            this.nom = nom;
            this.gen = gen;
            this.fecNac = fecNac;
            this.email = email;
            this.tel = tel;
            this.pswdHash = pswdHash;
            this.sedePref = sedePref;
            this.activo = activo;
            this.fechaCreacion = fechaCreacion;
            this.fechaUltimaActualizacion = fechaUltimaActualizacion;
        }

        public string Id { get => id; set => id = value; }
        public string IdRol { get => idRol; set => idRol = value; }
        public string TipoDoc { get => tipoDoc; set => tipoDoc = value; }
        public string NumDoc { get => numDoc; set => numDoc = value; }
        public string Ape { get => ape; set => ape = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Gen { get => gen; set => gen = value; }
        public DateTime? FecNac { get => fecNac; set => fecNac = value; }
        public string Email { get => email; set => email = value; }
        public string Tel { get => tel; set => tel = value; }
        public byte[] PswdHash { get => pswdHash; set => pswdHash = value; }
        public string SedePref { get => sedePref; set => sedePref = value; }
        public bool Activo { get => activo; set => activo = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public DateTime FechaUltimaActualizacion { get => fechaUltimaActualizacion; set => fechaUltimaActualizacion = value; }
    }
}
