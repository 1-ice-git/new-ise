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
                    cem.idCalendarioEventi = ca.IDCALENDARIOEVENTI;//per il ref parametro
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  dell'evento relativo al calendario eventi.",
                        "CALENDARIOEVENTI", db, ca.IDTRASFERIMENTO, ca.IDCALENDARIOEVENTI);
                }
            }
        }


        public void InsertCalendarioEvento(ref CalendarioEventiModel cem, ModelDBISE db)
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
                cem.idCalendarioEventi = ca.IDCALENDARIOEVENTI;//per il ref parametro
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  dell'evento relativo al calendario eventi.",
                    "CALENDARIOEVENTI", db, ca.IDTRASFERIMENTO, ca.IDCALENDARIOEVENTI);
            }

        }


        public void ModificaInCompletatoCalendarioEvento(decimal idTrasferimento, EnumFunzioniEventi fe)
        {
            //int funzEv = (int)fe;
            //using (ModelDBISE db = new ModelDBISE())
            //{
            //    var result = db.CALENDARIOEVENTI.Where(c => c.IDTRASFERIMENTO == idTrasferimento && c.IDFUNZIONIEVENTI == funzEv).ToList();
            //    foreach (var x in result)
            //    {
            //        CALENDARIOEVENTI y = db.CALENDARIOEVENTI.Find(x.IDCALENDARIOEVENTI);
            //        y.COMPLETATO = true;
            //        y.DATACOMPLETATO = DateTime.Now;
            //        int i = db.SaveChanges();
            //        if (i <= 0)
            //        {
            //            throw new Exception("Errore nella fase di modifica in 'Completato' dell'evento per il calendario eventi.");
            //        }
            //        else
            //        {
            //            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica in 'Completato' dell'evento relativo al calendario eventi.",
            //              "CALENDARIOEVENTI", db, x.IDTRASFERIMENTO, x.IDCALENDARIOEVENTI);
            //        }
            //    }
            //}
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal funzEv = (decimal)fe;
                var lce =
                    db.CALENDARIOEVENTI.Where(
                        c =>
                            c.IDTRASFERIMENTO == idTrasferimento && c.IDFUNZIONIEVENTI == funzEv && c.COMPLETATO == false &&
                            c.ANNULLATO == false).OrderByDescending(a => a.IDCALENDARIOEVENTI);

                if (lce?.Any() ?? false)
                {
                    CALENDARIOEVENTI y = lce.First();
                    y.COMPLETATO = true;
                    y.DATACOMPLETATO = DateTime.Now;
                    int i = db.SaveChanges();
                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di modifica in 'Completato' dell'evento per il calendario eventi.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica in 'Completato' dell'evento relativo al calendario eventi.",
                          "CALENDARIOEVENTI", db, idTrasferimento, y.IDCALENDARIOEVENTI);
                    }
                }
            }

        }
        public void ModificaInCompletatoCalendarioEvento(decimal idTrasferimento, EnumFunzioniEventi fe, ModelDBISE db)
        {
            decimal funzEv = (decimal)fe;
            var lce =
                db.CALENDARIOEVENTI.Where(
                    c =>
                        c.IDTRASFERIMENTO == idTrasferimento && c.IDFUNZIONIEVENTI == funzEv && c.COMPLETATO == false &&
                        c.ANNULLATO == false).OrderByDescending(a => a.IDCALENDARIOEVENTI);

            if (lce?.Any() ?? false)
            {
                CALENDARIOEVENTI y = lce.First();
                y.COMPLETATO = true;
                y.DATACOMPLETATO = DateTime.Now;
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di modifica in 'Completato' dell'evento per il calendario eventi.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica in 'Completato' dell'evento relativo al calendario eventi.",
                      "CALENDARIOEVENTI", db, idTrasferimento, y.IDCALENDARIOEVENTI);
                }
            }

        }

        public void AnnullaMessaggioEvento(decimal idTrasferimento, EnumFunzioniEventi fe, ModelDBISE db)
        {
            decimal funzEv = (decimal)fe;
            var lce =
                db.CALENDARIOEVENTI.Where(
                    a =>
                        a.IDTRASFERIMENTO == idTrasferimento && a.IDFUNZIONIEVENTI == funzEv && a.COMPLETATO == false &&
                        a.ANNULLATO == false).OrderByDescending(a => a.IDCALENDARIOEVENTI);

            if (lce?.Any() ?? false)
            {
                CALENDARIOEVENTI y = lce.First();
                y.ANNULLATO = true;
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di modifica in 'Completato' dell'evento per il calendario eventi.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Annullato, "Annullamento del evento.",
                      "CALENDARIOEVENTI", db, idTrasferimento, y.IDCALENDARIOEVENTI);
                }
            }

        }

        public List<ElencoElementiHome> GetListaElementiHome()
        {
            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            List<ElencoElementiHome> tmp1 = new List<ElencoElementiHome>();
            List<ElencoElementiHome> tmpAll = new List<ElencoElementiHome>();
            AccountModel am = new AccountModel();

            try
            {

                bool admin = Utility.Amministratore(out am);


                using (ModelDBISE db = new ModelDBISE())
                {
                    if (admin)
                    {
                        tmp = (from e in db.CALENDARIOEVENTI
                               where e.COMPLETATO == true &&
                                     e.DATACOMPLETATO.Month == DateTime.Now.Month &&
                                     e.DATACOMPLETATO.Year == DateTime.Now.Year &&
                                     e.ANNULLATO == false
                               orderby e.DATACOMPLETATO descending
                               select new ElencoElementiHome()
                               {
                                   IdFunzioneEvento = e.IDFUNZIONIEVENTI,
                                   dataInizio = e.DATAINIZIOEVENTO,
                                   dataScadenza = e.DATASCADENZA,
                                   NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                                   Completato = e.COMPLETATO,
                                   Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                                   IdDipendente = e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE,
                               }).ToList();

                        tmp1 = (from e in db.CALENDARIOEVENTI
                                where e.COMPLETATO == false && e.ANNULLATO == false
                                orderby e.DATASCADENZA descending
                                orderby e.DATAINIZIOEVENTO descending
                                select new ElencoElementiHome()
                                {
                                    IdFunzioneEvento = e.IDFUNZIONIEVENTI,
                                    dataInizio = e.DATAINIZIOEVENTO,
                                    dataScadenza = e.DATASCADENZA,
                                    NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                                    Completato = e.COMPLETATO,
                                    Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                                    IdDipendente = e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE,
                                }).ToList();
                    }
                    else
                    {
                        tmp = (from e in db.CALENDARIOEVENTI
                               where e.COMPLETATO == true &&
                                     e.DATACOMPLETATO.Month == DateTime.Now.Month &&
                                     e.DATACOMPLETATO.Year == DateTime.Now.Year &&
                                     e.ANNULLATO == false &&
                                     e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE == am.idDipendente
                               orderby e.DATACOMPLETATO descending
                               select new ElencoElementiHome()
                               {
                                   IdFunzioneEvento = e.IDFUNZIONIEVENTI,
                                   dataInizio = e.DATAINIZIOEVENTO,
                                   dataScadenza = e.DATASCADENZA,
                                   NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                                   Completato = e.COMPLETATO,
                                   Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                                   IdDipendente = e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE,
                               }).ToList();

                        tmp1 = (from e in db.CALENDARIOEVENTI
                                where e.COMPLETATO == false && e.ANNULLATO == false && e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE == am.idDipendente
                                orderby e.DATASCADENZA descending
                                orderby e.DATAINIZIOEVENTO descending
                                select new ElencoElementiHome()
                                {
                                    IdFunzioneEvento = e.IDFUNZIONIEVENTI,
                                    dataInizio = e.DATAINIZIOEVENTO,
                                    dataScadenza = e.DATASCADENZA,
                                    NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                                    Completato = e.COMPLETATO,
                                    Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                                    IdDipendente = e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE,
                                }).ToList();
                    }

                    tmpAll.AddRange(tmp);
                    tmpAll.AddRange(tmp1);
                }


                return (tmpAll);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DettagliMessaggio OgggettoFunzioneEvento(EnumFunzioniEventi idf, int idd)
        {
            List<DettagliMessaggio> tmp = new List<DettagliMessaggio>();
            try
            {
                using (var db = new ModelDBISE())
                {
                    decimal x = (decimal)idf;
                    tmp = (from e in db.CALENDARIOEVENTI
                           where e.IDFUNZIONIEVENTI == x && e.TRASFERIMENTO.IDDIPENDENTE == idd && e.ANNULLATO == false
                           select new DettagliMessaggio()
                           {
                               NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                               Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                               Trasferimento = e.TRASFERIMENTO.UFFICI.DESCRIZIONEUFFICIO + " (" + e.TRASFERIMENTO.UFFICI.CODICEUFFICIO + ")",
                               MessaggioEvento = e.FUNZIONIEVENTI.MESSAGGIOEVENTO,
                           }).ToList();
                }
                return tmp.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}