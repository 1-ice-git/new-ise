using NewISE.EF;
using NewISE.Models.Tools;
using System;
using NewISE.Models.DBModel.dtObj;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{


    public class PagatoCondivisoMABModel
    {
        [Key]
        public decimal idPagatoCondiviso { get; set; }

        public decimal idMAB { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizioValidita { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data Fine")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFineValidita { get; set; }

        public bool Condiviso { get; set; }

        public bool Pagato { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public decimal idStatoRecord { get; set; }

        public decimal? fk_IDPagatoCondiviso { get; set; }

        public bool Nascondi { get; set; }

        public bool HasValue()
        {
            return idPagatoCondiviso > 0 ? true : false;
        }

        public void Annulla(ModelDBISE db)
        {
            var pcmab = db.PAGATOCONDIVISOMAB.Find(this.idPagatoCondiviso);
            if (pcmab != null && pcmab.IDPAGATOCONDIVISO > 0)
            {
                pcmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                if (db.SaveChanges() > 0)
                {
                    decimal idTrasf = pcmab.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica del pagato condiviso", "PAGATOCONDIVISO", db, idTrasf, pcmab.IDPAGATOCONDIVISO);
                }
            }
        }


        public void NascondiRecord(ModelDBISE db)
        {
            var pcmab = db.PAGATOCONDIVISOMAB.Find(this.idPagatoCondiviso);
            if (pcmab != null && pcmab.IDPAGATOCONDIVISO > 0)
            {
                pcmab.NASCONDI = true;
                db.SaveChanges();
            }
        }

        public void MostraRecord(ModelDBISE db)
        {
            var pcmab = db.PAGATOCONDIVISOMAB.Find(this.idPagatoCondiviso);
            if (pcmab != null && pcmab.IDPAGATOCONDIVISO > 0)
            {
                pcmab.NASCONDI = false;
                db.SaveChanges();
            }
        }

    }
}