using control_notas_cit.Models.Entidades;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class CalendarioViewModel
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name="Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }
        [Required]
        public List<Semana> Semanas { get; set; }
    }

    public class SemanaViewModel
    {
        [Required]
        public int SemanaID { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Actividad { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Fecha { get; set; }
        public int NumeroSemana { get; set; }
    }
}