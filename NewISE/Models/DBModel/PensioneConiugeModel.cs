using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel
{
    public class PensioneConiugeModel
    {
        [Key]
        public decimal idPensioneConiuge { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "L'importo della pensione è richiesto.")]
        [Display(Name = "Pensione")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal importoPensione { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.", AutoGenerateField = false)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [ScaffoldColumn(false)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato", AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public bool annullato { get; set; }

        public IList<ConiugeModel> Coniugi { get; set; }


        public bool HasValue()
        {
            return idPensioneConiuge > 0 ? true : false;
        }

        public void Annulla(ModelDBISE db)
        {
            var p = db.PENSIONE.Find(this.idPensioneConiuge);
            if (p != null && p.IDPENSIONE > 0)
            {
                p.ANNULLATO = true;


                int i = db.SaveChanges();

                if (i > 0)
                {
                    decimal idTrasf = p.CONIUGE.First().MAGGIORAZIONIFAMILIARI.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica della pensione", "PENSIONE", db, idTrasf, p.IDPENSIONE);
                }


            }
        }



    }
}