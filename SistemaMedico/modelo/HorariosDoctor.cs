using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaMedico.modelo
{
    public class HorariosDoctor
    {
        private string id;
        private string idDoc;
        private string idSede;
        private int diaSem;
        private TimeSpan hora;
        private TimeSpan horaFin;

        public HorariosDoctor()
        {
            this.id = "";
            this.idDoc = "";
            this.idSede = "";
            this.diaSem = 1;
            this.hora = TimeSpan.Zero;
            this.horaFin = TimeSpan.Zero;
        }

        public HorariosDoctor(string id, string idDoc, string idSede, int diaSem, TimeSpan hora, TimeSpan horaFin)
        {
            this.id = id;
            this.idDoc = idDoc;
            this.idSede = idSede;
            this.diaSem = diaSem;
            this.hora = hora;
            this.horaFin = horaFin;
        }

        public string Id { get => id; set => id = value; }
        public string IdDoc { get => idDoc; set => idDoc = value; }
        public string IdSede { get => idSede; set => idSede = value; }
        public int DiaSem { get => diaSem; set => diaSem = value; }
        public TimeSpan Hora { get => hora; set => hora = value; }
        public TimeSpan HoraFin { get => horaFin; set => horaFin = value; }
    }
}
