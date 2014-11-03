using control_notas_cit.Models.Entidades;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace control_notas_cit.Models.ViewModels
{
    public class ProfesorIndexViewModel
    {
        public string NombreProfesor { get; set; }
        public Proyecto Proyecto { get; set; }
        public Calendario Calendario { get; set; }
    }
}