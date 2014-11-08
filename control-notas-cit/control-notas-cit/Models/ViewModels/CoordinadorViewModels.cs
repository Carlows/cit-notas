using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using control_notas_cit.Models.Entidades;
using System.ComponentModel.DataAnnotations;

namespace control_notas_cit.Models.ViewModels
{
    public class CoordinadorIndexViewModel
    {
        public Celula Celula { get; set; }
        public Semana Semana { get; set; }
    }
}