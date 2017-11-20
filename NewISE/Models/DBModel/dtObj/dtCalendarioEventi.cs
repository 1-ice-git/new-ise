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
        public void ModificaInCompletatoCalendarioEvento(decimal idTrasferimento, EnumFunzioniEventi fe)
        {
            int funzEv = (int)fe;
            using (ModelDBISE db = new ModelDBISE())
            {
                var result = db.CALENDARIOEVENTI.Where(c => c.IDTRASFERIMENTO== idTrasferimento && c.IDFUNZIONIEVENTI==funzEv).ToList();
                foreach (var x in result)
                {
                    CALENDARIOEVENTI y = db.CALENDARIOEVENTI.Find(x.IDCALENDARIOEVENTI);
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
                          "CALENDARIOEVENTI", db, x.IDTRASFERIMENTO, x.IDCALENDARIOEVENTI);
                    }
                }
            }
        }
        public void ModificaInCompletatoCalendarioEvento(decimal idTrasferimento, EnumFunzioniEventi fe, ModelDBISE db)
        {
                int funzEv = (int)fe;            
                var result = db.CALENDARIOEVENTI.Where(c => c.IDTRASFERIMENTO == idTrasferimento && c.IDFUNZIONIEVENTI == funzEv).ToList();
                foreach (var x in result)
                {
                    CALENDARIOEVENTI y = db.CALENDARIOEVENTI.Find(x.IDCALENDARIOEVENTI);
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
                          "CALENDARIOEVENTI", db, x.IDTRASFERIMENTO, x.IDCALENDARIOEVENTI);
                    }
                }
        }
        
        public List<ElencoElementiHome> GetListaElementiHome()
        {
            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            List<ElencoElementiHome> tmp1 = new List<ElencoElementiHome>();
            List<ElencoElementiHome> tmpAll = new List<ElencoElementiHome>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    tmp = (from e in db.CALENDARIOEVENTI
                           where e.COMPLETATO == true && 
                                 e.DATACOMPLETATO.Month == DateTime.Now.Month && 
                                 e.DATACOMPLETATO.Year == DateTime.Now.Year
                                 orderby e.DATACOMPLETATO descending
                           select new ElencoElementiHome()
                           {
                               IdFunzioneEvento = e.IDFUNZIONIEVENTI,
                               dataInizio = e.DATAINIZIOEVENTO,
                               dataScadenza = e.DATASCADENZA,
                               NomeFunzione = e.FUNZIONIEVENTI.NOMEFUNZIONE,
                               Completato = e.COMPLETATO,
                               Nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                               IdDipendente=e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE,
                           }).ToList();

                    tmp1 = (from e in db.CALENDARIOEVENTI
                            where e.COMPLETATO == false
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

        public DettagliMessaggio OgggettoFunzioneEvento(EnumFunzioniEventi idf,int idd)
        {
            List <DettagliMessaggio> tmp = new List<DettagliMessaggio>();           
            try
            {
                using (var db = new ModelDBISE())
                {
                    int x = (int)idf;
                    tmp = (from e in db.CALENDARIOEVENTI
                           where e.IDFUNZIONIEVENTI == x && e.TRASFERIMENTO.IDDIPENDENTE==idd
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
        public List<CalendarViewModel> GetConteggioStatiAttivita(DateTime inizio)
        {
            List<CalendarViewModel> x = new List<CalendarViewModel>();            

            //DateTime corrente;

            List<CalendarViewModel> ElencoAttivita = new List<CalendarViewModel>();

            CalendarViewModel attivi = new CalendarViewModel();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    //var la = db.CALENDARIOEVENTI.Where(a=>a.ANNULLATO == false && inizio >= a.DATAINIZIOEVENTO && inizio <= a.DATASCADENZA && a.COMPLETATO == false).ToList();

                    var la = db.CALENDARIOEVENTI.Where(a => a.ANNULLATO == false  && a.COMPLETATO == false).ToList();
                    la = la.Where(a => inizio.Date >= a.DATAINIZIOEVENTO.Date && inizio <= a.DATASCADENZA.Value.Date).ToList();
                    //if (la?.Any() ?? false)
                    //{
                    //    var n = la.Count;

                    //    var prova = la.Where(a => inizio.Date >= a.DATAINIZIOEVENTO.Date && inizio.Date <= a.DATASCADENZA.Value.Date).ToList();
                    //    var prova2 = la.Where(a => inizio >= a.DATAINIZIOEVENTO && inizio <= a.DATASCADENZA.Value).ToList();
                    //    var prova3 = la.Where(a => a.DATAINIZIOEVENTO <= inizio && a.DATASCADENZA.Value >= inizio).ToList();
                    //    var prova4 = la.Where(a => a.DATAINIZIOEVENTO.Date <= inizio.Date && a.DATASCADENZA.Value.Date >= inizio.Date).ToList();

                    //}
                    var numeroAttivi = la.Count;
                    int meseCorrente=inizio.Month,annoCorrente=inizio.Year;
                    DateTime attuale;
                    if (inizio.Day != 1)
                    {
                        attuale= inizio.AddMonths(1);
                        meseCorrente = attuale.Month;
                        annoCorrente = attuale.Year;
                    }
                    
                   

                    string StringDate = string.Format("{0:yyyy-MM-dd}", inizio.Date);
                    //string StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
                    // string EndDateString = StringDate + "T23:59:59";

                    attivi = new CalendarViewModel()
                    {
                        title = "Attivi: " + numeroAttivi.ToString(),// + "\nCompletati:" + numeroCompletati.ToString() + "\nScaduti: " + numeroScaduti.ToString(),
                       // Completati = "Completati: " + numeroCompletati.ToString(),
                        start = StringDate,
                        end = StringDate,
                        color="blue"
                    };

                    ElencoAttivita.Add(attivi);

                    CalendarViewModel completati = new CalendarViewModel();
                    var lc = db.CALENDARIOEVENTI.Where(a => a.ANNULLATO == false && a.COMPLETATO == true &&
                    a.DATAINIZIOEVENTO.Month == meseCorrente &&
                    a.DATAINIZIOEVENTO.Year == annoCorrente).ToList();
                    var numeroCompletati = lc.Count;
                    completati = new CalendarViewModel()
                    {
                        title = "Completati: " + numeroCompletati.ToString(),
                        start = StringDate,
                        end = StringDate,
                        color="green"
                    };
                    ElencoAttivita.Add(completati);

                    CalendarViewModel scaduti = new CalendarViewModel();
                    var ls = db.CALENDARIOEVENTI.Where(a => a.ANNULLATO == false && a.COMPLETATO == false &&
                    a.DATASCADENZA.Value < inizio).ToList();
                    var numeroScaduti = ls.Count;
                    scaduti = new CalendarViewModel()
                    {
                        title = "Scaduti: " + numeroScaduti.ToString(),
                        start = StringDate,
                        end = StringDate,
                        color="red"
                    };
                    ElencoAttivita.Add(scaduti);
                }

                return ElencoAttivita;

                //using (ModelDBISE db = new ModelDBISE())
                //{
                //    int contaAtt = db.CALENDARIOEVENTI
                //       .Where(p => p.COMPLETATO == false && p.DATAINIZIOEVENTO >= inizio)
                //       .Select(g => new { }).Count();
                //    int a = contaAtt;
                //    string titolo = "Attivi " + a.ToString();

                //    attivi = (from e in db.CALENDARIOEVENTI
                //              where e.COMPLETATO == false && e.DATAINIZIOEVENTO >= inizio
                //              select new CalendarViewModel()
                //              {
                //                  id = e.IDCALENDARIOEVENTI,
                //                  title = titolo,
                //                  start = (DateTime)e.DATAINIZIOEVENTO,
                //                  //   end=(DateTime)e.DATASCADENZA,                                  
                //              }).Distinct().ToList();
                //    x.AddRange(attivi);

                //    int contaComp = db.CALENDARIOEVENTI
                //       .Where(p => p.COMPLETATO == true && p.DATAINIZIOEVENTO.Month == inizio.Month && p.DATAINIZIOEVENTO.Year == inizio.Year)
                //       .GroupBy(f => f.COMPLETATO)
                //       .Select(g => new { }).Count();
                //    int c = contaComp;
                //    titolo = "Completi " + c.ToString();
                //    //tmp = new CalendarViewModel();
                //    //tmp.title = titolo; tmp.id = DateTime.Now.Ticks;
                //    //x.Add(tmp);
                //    completati = (from e in db.CALENDARIOEVENTI
                //                  where e.COMPLETATO == true
                //                  select new CalendarViewModel()
                //                  {
                //                      id = e.IDCALENDARIOEVENTI,
                //                      title = titolo,
                //                      start = (DateTime)e.DATAINIZIOEVENTO,
                //                      //   end = (DateTime)e.DATASCADENZA,
                //                  }).Distinct().ToList();
                //    x.AddRange(completati);

                //    //int contaScad = db.CALENDARIOEVENTI
                //    //  .Where(p => p.COMPLETATO == false && p.DATAINIZIOEVENTO< inizio)
                //    //  .GroupBy(f => f.IDCALENDARIOEVENTI)
                //    //  .Select(g => new { u = g.Count(), }).Count();
                //    //int s = contaScad;
                //    //titolo = "Scaduti " + s.ToString();
                //    //tmp = new CalendarViewModel();
                //    //tmp.title = titolo; tmp.id = DateTime.Now.Ticks;
                //    //x.Add(tmp);
                //    ////scaduti = (from e in db.CALENDARIOEVENTI
                //    ////              where e.COMPLETATO == false &&  e.DATASCADENZA< inizio
                //    ////              select new CalendarViewModel()
                //    ////              {
                //    ////                  id = e.IDCALENDARIOEVENTI,
                //    ////                //  title =  titolo,
                //    ////                  start = (DateTime)e.DATAINIZIOEVENTO,
                //    ////                 // end = (DateTime)e.DATASCADENZA,
                //    ////              }).ToList();
                //    ////x.AddRange(scaduti);

                //    return x;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    } 
}