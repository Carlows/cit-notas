using control_notas_cit.Models.Entidades;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    public class CelulaViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name="Coordinadores")]
        public List<string> CoordinadoresID { get; set; }

        public MultiSelectList Coordinadores { get; set; }
    }

    public class CelulaEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
    }

    public class CoordinadorViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage="El email es requerido.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "La cedula es requerida.")]
        public string Cedula { get; set; }
        [Required(ErrorMessage="La contraseña es requerida.")]
        [Display(Name="Contraseña")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage="El telefono es requerido.")]
        public string Telefono { get; set; }

        public string Celula { get; set; }

        public SelectList Celulas { get; set; }
    }

    public class CoordinadorEditViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "La cedula es requerida.")]
        public string Cedula { get; set; }
        [Required(ErrorMessage = "El telefono es requerido.")]
        public string Telefono { get; set; }
        [Display(Name="Celula")]
        public string CelulaID { get; set; }

        public SelectList Celulas { get; set; }
    }
}