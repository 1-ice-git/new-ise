using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class DipendentiModel
    {
        [Key]
        public decimal idDipendente { get; set; }
        [Required(ErrorMessage = "La matricola è richiesta.")]
        [Display(Name = "Matricola")]
        [Range(1, 999999, ErrorMessage = "Sono consentiti valori da 1 a 999999")]
        [CustomValidation(typeof(dtDipendenti), "MatricolaUnivoca", ErrorMessage = "La matricola inserita è già presente, inserirne un altra.")]
        public int matricola { get; set; }
        [Required(ErrorMessage = "Il nome è richiesto.")]
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il nome accetta un massimo di 30 caratteri.")]
        [Display(Name = "Nome")]
        public string nome { get; set; }
        [Required(ErrorMessage = "Il cognome è richiesto.")]
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il cognome accetta un massimo di 30 caratteri.")]
        [Display(Name = "Cognome")]
        public string cognome { get; set; }
        [Required(ErrorMessage = "La data di assunzione è richiesta.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data assunzione")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime dataAssunzione { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Data assunzione")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? dataCessazione { get; set; }
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "L'indirizzo accetta un massimo di 100 caratteri.")]
        [Display(Name = "Indirizzo")]
        public string indirizzo { get; set; }
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Il campo CAP accetta un massimo di 10 caratteri.")]
        [Display(Name = "CAP")]
        public string cap { get; set; }
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il campo città accetta un massimo di 30 caratteri.")]
        [Display(Name = "Città")]
        public string citta { get; set; }
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il campo provincia accetta un massimo di 30 caratteri.")]
        [Display(Name = "Provincia")]
        public string provincia { get; set; }
        [EmailAddress(ErrorMessage = "L'e-mail non è valida.")]
        [StringLength(50, ErrorMessage = "Il campo e-mail accetta un massimo di 50 caratteri.")]
        [Display(Name = "E-mail")]
        [CustomValidation(typeof(dtDipendenti), "EmailUnivoca", ErrorMessage = "L'E-mail inserita è già presente, inserirne un altra.")]
        public string email { get; set; }
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il campo telefono accetta un massimo di 30 caratteri.")]
        [Display(Name = "Telefono")]
        public string telefono { get; set; }
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "Il campo fax accetta un massimo di 30 caratteri.")]
        [Display(Name = "Fax")]
        public string fax { get; set; }

        [Required(ErrorMessage = "La data d'inizio ricalcoli è richiesta.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data assunzione")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? dataInizioRicalcoli { get; set; }

        [Display(Name = "Abilitato")]
        public bool abilitato { get; set; }


        public string Nominativo
        {
            get
            {
                return cognome + " " + nome;
            }
        }

        public bool HasValue()
        {
            return this.idDipendente > 0 ? true : false;
        }

        public IList<LivelloDipendenteModel> LivelloDipendenti { get; set; }

        public IList<CDCDipendentiModel> CdcDipendenti { get; set; }

        public CDCGepeModel cdcGepe { get; set; }

        public LivelloDipendenteModel livelloDipendenteValido { get; set; }

        public UtenteAutorizzatoModel UtenteAutorizzato { get; set; }


    }
}