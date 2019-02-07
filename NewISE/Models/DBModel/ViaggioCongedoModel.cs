using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ViaggioCongedoModel
    {
        [Key]
        public decimal idViaggioCongedo { get; set; }
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "Il nome del file è richiesto.")]
        [Display(Name = "Nome File")]
        public string NomeFile { get; set; }
        public decimal idTipoDocumento { get; set; }
        public decimal idAttivazioneVC { get; set; }
        public bool NotificaRichiesta { get; set; }
        public bool AttivaRichiesta { get; set; }
        public decimal idDocumento { get; set; }
        public string Estensione { get; set; }
        public bool DocSelezionato { get; set; }
        public decimal idStatoRecord { get; set; }
        public bool HasValue()
        {
            return idTrasferimento > 0 ? true : false;
        }
    }
}