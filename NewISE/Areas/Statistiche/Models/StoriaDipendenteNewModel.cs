using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class StoriaDipendenteNewModel
    {
        public decimal idDipendente { get; set; }
        public int matricola { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public DateTime dataAssunzione { get; set; }
        public DateTime? dataCessazione { get; set; }
        public string indirizzo { get; set; }
        public string cap { get; set; }
        public string citta { get; set; }
        public string provincia { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string fax { get; set; }
        public DateTime? dataInizioRicalcoli { get; set; }
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

        public LivelloDipendenteModel Livello { get; set; }

        public decimal IdLivello { get; set; }


        public IList<LivelloDipendenteModel> LivelloDipendenti { get; set; }

        public IList<CDCDipendentiModel> CdcDipendenti { get; set; }

        public CDCGepeModel cdcGepe { get; set; }

        public LivelloDipendenteModel livelloDipendenteValido { get; set; }

        public UtenteAutorizzatoModel UtenteAutorizzato { get; set; }

        public IList<ElaborazioniModel> lElaborazioni { get; set; }

        public string Ufficio { get; set; }

        public string DescLivello { get; set; }

        public string ValutaUfficio { get; set; }

        public decimal indennita { get; set; }

        public DateTime dataPartenza { get; set; }

        public DateTime dataVariazione { get; set; }

        public DateTime dataRientro { get; set; }

        public decimal valore { get; set; }

        public decimal percentuale { get; set; }
        
        public DateTime? dataLettera { get; set; }

        public decimal IndennitaBase { get; set; }
        public decimal IndennitaServizio { get; set; }
        public decimal IndennitaRichiamo { get; set; }
        public decimal IndennitaPersonale { get; set; }
        public decimal AnticipoPrimaSistemazione { get; set; }
        public decimal PensioneConiuge { get; set; }
        public decimal MaggiorazioniFamiliari { get; set; }
        public decimal PrimaSistemazione { get; set; }
        
    }
}