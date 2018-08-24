using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{
    public class CanoneMABModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCanone { get; set; }

        public decimal IDAttivazioneMAB { get; set; }

        public decimal IDMAB { get; set; }

        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizioValidita { get; set; }

        [Display(Name = "Data Fine")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFineValidita { get; set; }

        [Display(Name = "Canone")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal ImportoCanone { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public decimal idStatoRecord { get; set; }

        public decimal? FK_IDCanone { get; set; }

        public bool HasValue()
        {
            return idCanone > 0 ? true : false;
        }

        public TFRModel TFR;

        public decimal idValuta { get; set; }

        [Display(Name = "Aggiorna Tutto")]
        public bool chkAggiornaTutti { get; set; }

        public bool nascondi { get; set; }

        
        public void Annulla(ModelDBISE db)
        {
            var cmab = db.CANONEMAB.Find(this.idCanone);
            if (cmab != null && cmab.IDCANONE > 0)
            {
                cmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                if (db.SaveChanges() > 0)
                {
                    decimal idTrasf = cmab.MAB.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica del canone", "CANONEMAB", db, idTrasf, cmab.IDCANONE);
                }
            }
        }


        public void NascondiRecord(ModelDBISE db)
        {
            var cmab = db.CANONEMAB.Find(this.idCanone);
            if (cmab != null && cmab.IDCANONE > 0)
            {
                cmab.NASCONDI = true;
                db.SaveChanges();
            }
        }

        public void MostraRecord(ModelDBISE db)
        {
            var cmab = db.CANONEMAB.Find(this.idCanone);
            if (cmab != null && cmab.IDCANONE > 0)
            {
                cmab.NASCONDI = false;
                db.SaveChanges();
            }
        }

    }
}