using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class EmailSecondarieDipModel
    {
        [Key]
        public decimal idEmailSecDip { get; set; }
        public decimal idDipendente { get; set; }
        public string Email { get; set; }
        public bool  Attiva { get; set; }
    }
}