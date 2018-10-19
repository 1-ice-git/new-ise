using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Statistiche.Models
{
    public class RptRiepiloghiIseMensileModel
    {
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Qualifica")]
        public string qualifica { get; set; }
        [Display(Name = "Sede")]
        public string sede { get; set; }
        [Display(Name = "Valuta")]
        public string valuta { get; set; }
        [Display(Name = "Codice Tipo Movimento")]
        public string codice_tipo_movimento { get; set; }
        [Display(Name = "Tipo Movimento")]
        public string tipo_movimento { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string data_decorrenza { get; set; }
        [Display(Name = "Data Lettera")]
        public string data_lettera { get; set; }
        [Display(Name = "Data Operazione")]
        public string data_operazione { get; set; }
        [Display(Name = "Indennità Personale")]
        public string indennita_personale { get; set; }
        [Display(Name = "Sist. /Rientro Lorda")]
        public string anticipo { get; set; }

        public decimal meseRiferimento { get; set; }

        public decimal IdTeorici { get; set; }
        public string codiceVoce { get; set; }
        public string DescrizioneVoce { get; set; }
        public string Voce { get; set; }
        public string Movimento { get; set; }
        public string Inserimento { get; set; }
        public string Liquidazione { get; set; }
        public string DataRiferimento { get; set; }
        public decimal Importo { get; set; }
        public bool Inviato { get; set; }
        public string Nominativo { get; set; }

        public string Ufficio { get; set; }

        public TipoMovimentoModel TipoMovimento { get; set; }

        [Display(Name = "Movimentazione")]
        public EnumTipoInserimento tipoInserimento { get; set; }

        [Display(Name = "Prima Sistemazione")]
        public string prima_sistemazione { get; set; }

        [Display(Name = "Richiamo")]
        public string richiamo { get; set; }

        public decimal mese { get; set; }

        public decimal anno { get; set; }

        public string riferimento { get; set; }

        public string elaborazione { get; set; }
    }
}