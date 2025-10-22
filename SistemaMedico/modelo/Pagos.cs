using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class Pagos
    {
        private string id;
        private string idCita;
        private decimal monto;
        private string metPago;
        private string estPago;
        private string idTrans;
        private DateTime? fec;

        public Pagos()
        {
            this.id = "";
            this.idCita = "";
            this.monto = 0;
            this.metPago = "";
            this.estPago = "Pendiente";
            this.idTrans = "";
            this.fec = DateTime.Now;
        }

        public Pagos(string id, string idCita, decimal monto, string metPago, 
                    string estPago, string idTrans, DateTime? fec)
        {
            this.id = id;
            this.idCita = idCita;
            this.monto = monto;
            this.metPago = metPago;
            this.estPago = estPago;
            this.idTrans = idTrans;
            this.fec = fec;
        }

        public string Id { get => id; set => id = value; }
        public string IdCita { get => idCita; set => idCita = value; }
        public decimal Monto { get => monto; set => monto = value; }
        public string MetPago { get => metPago; set => metPago = value; }
        public string EstPago { get => estPago; set => estPago = value; }
        public string IdTrans { get => idTrans; set => idTrans = value; }
        public DateTime? Fec { get => fec; set => fec = value; }
    }
}
