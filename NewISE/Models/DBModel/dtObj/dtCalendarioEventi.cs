using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtCalendarioEventi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InsertCalendarioEvento(ref CalendarioEventiModel cem)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                CALENDARIOEVENTI ca = new CALENDARIOEVENTI();

                ca.ANNULLATO = cem.Annullato;
                ca.COMPLETATO = cem.Completato;
                ca.DATACOMPLETATO = cem.DataCompletato;
                ca.DATAINIZIOEVENTO = cem.DataInizioEvento;
                ca.DATASCADENZA = cem.DataScadenza;
                ca.IDFUNZIONIEVENTI = (decimal)cem.idFunzioneEventi;
                ca.IDTRASFERIMENTO = cem.idTrasferimento;
                db.CALENDARIOEVENTI.Add(ca);
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception("Errore nella fase d'inserimento dell'evento per il calendario eventi.");
                }
                else
                {
                    cem.idCalendarioEventi = ca.IDCALENDARIOEVENTI;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  dell'evento relativo al calendario eventi.",
                        "CALENDARIOEVENTI", db, ca.IDTRASFERIMENTO, ca.IDCALENDARIOEVENTI);
                }
            }
        }
        public void ModificaInCompletatoCalendarioEvento(decimal idTrasferimento,FunzioniEventi fe)
        {
            int funzEv = (int)fe;
            using (ModelDBISE db = new ModelDBISE())
            {
                var result = db.CALENDARIOEVENTI.Where(c => c.IDTRASFERIMENTO== idTrasferimento && c.IDFUNZIONIEVENTI==funzEv);
                foreach (var x in result)
                {
                    x.COMPLETATO = true;
                    x.DATACOMPLETATO = DateTime.Now;
                    int i = db.SaveChanges();
                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di modifica in 'Completato' dell'evento per il calendario eventi.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica in 'Completato' dell'evento relativo al calendario eventi.",
                          "CALENDARIOEVENTI", db, x.IDTRASFERIMENTO, x.IDCALENDARIOEVENTI);
                    }
                }
            }
        }
        
    }
}